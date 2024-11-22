using MD_Tech.Context;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly MdtecnologiaContext mdtecnologiaContext;
        private readonly LogsApi<ProveedoresController> logger;

        public ProveedoresController(MdtecnologiaContext mdtecnologiaContext, LogsApi<ProveedoresController> logger)
        {
            this.mdtecnologiaContext = mdtecnologiaContext;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene todos los proveedores", Description = "Devuelve una lista de proveedores")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<ProveedoresDto>))]
        public async Task<ActionResult<List<ProveedoresDto>>> GetProveedor()
        {
            return Ok(new
            {
                proveedores = await mdtecnologiaContext.Proveedores
                    .AsNoTracking()
                    .Include(p => p.DireccionNavigation)
                    .Select(p => new ProveedoresDto(p))
                    .ToListAsync()
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene un proveedor por ID", Description = "Devuelve el detalle del proveedor")]
        [SwaggerResponse(200, "Operación exitosa", typeof(ProveedoresDto))]
        [SwaggerResponse(404, "Proveedor no encontrado")]
        public async Task<ActionResult<ProveedoresDto>> GetProveedor(Guid id)
        {
            var proveedor = await mdtecnologiaContext.Proveedores.FindAsync(id);
            return proveedor == null ? NotFound() : Ok(new { proveedor = new ProveedoresDto(proveedor) });
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Crea un proveedor", Description = "Agrega un nuevo proveedor a la base de datos")]
        [SwaggerResponse(201, "Proveedor creado", typeof(ProveedoresDto))]
        [SwaggerResponseHeader(201, "location", "string", "Enlace al recurso creado")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<ProveedoresDto>> AgregarProveedor([FromBody] ProveedoresDto proveedorDto)
        {
            try
            {
                var result = await ValidarProveedor(proveedorDto);
                if (result != null)
                    return result;

                var proveedor = await CrearProveedor(proveedorDto);
                await mdtecnologiaContext.SaveChangesAsync();
                return Created(
                    Url.Action(nameof(GetProveedor), "Proveedores", new { id = proveedor.Id }, Request.Scheme),
                    new { proveedor = new ProveedoresDto(proveedor), });
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Error al Crear un provedor");
                return Problem();
            }
        }

        [HttpPost("direccion")]
        [Authorize]
        [SwaggerOperation(Summary = "Crea un proveedor con su dirección", Description = "Agrega un nuevo proveedor con su dirección a la base de datos")]
        [SwaggerResponse(201, "Proveedor creado", typeof(ProveedoresDto))]
        [SwaggerResponseHeader(201, "location", "string", "Enlace al recurso creado")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<ProveedoresDto>> CrearProveedorDireccion([FromBody] ProveedoresDto proveedorDto)
        {
            var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            Direccion? direccion = null;
            try
            {
                if (proveedorDto.Direccion != null)
                {
                    if (proveedorDto.Direccion.Id != null && await mdtecnologiaContext.Direcciones.AnyAsync(d => d.Id == proveedorDto.Direccion.Id))
                    {
                        logger.Errores("Dirección Ya Registrada");
                        return BadRequest(new
                        {
                            direccion_Id = "Id en uso, si no desea crear una dirección nueva utilizar otro endpoint"
                        });
                    }

                    if (proveedorDto.Direccion.Provincia == null)
                        return BadRequest(new
                        { direccion_Provincia = "la provincia es requerida para crear una dirección" });
                    if (!await mdtecnologiaContext.Provincias.AnyAsync(p => p.Id == proveedorDto.Direccion.Provincia))
                        return BadRequest(new { direccion_Provincia = "provincia no es válida, intente nuevamente" });
                    direccion = new Direccion()
                    {
                        Id = proveedorDto.Direccion.Id ?? Guid.NewGuid(),
                        Provincia = (int)proveedorDto.Direccion.Provincia,
                        Descripcion = string.IsNullOrWhiteSpace(proveedorDto.Direccion.Descripcion) ? null : proveedorDto.Direccion.Descripcion,
                    };
                    await mdtecnologiaContext.Direcciones.AddAsync(direccion);
                    logger.Informacion("Se ha creado una Dirección");
                }
                // Se actualiza la referencia de direcciones
                proveedorDto.Direccion = new DireccionDto() { Id = direccion?.Id };
                await mdtecnologiaContext.SaveChangesAsync();
                // Se valida el resto de campos
                var result = await ValidarProveedor(proveedorDto);
                if (result != null)
                    return result;

                var proveedor = await CrearProveedor(proveedorDto);
                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Created(
                    Url.Action(nameof(GetProveedor), "Proveedores", new { id = proveedor.Id }, Request.Scheme),
                    new { proveedor = new ProveedoresDto(proveedor) });
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Error al crear Proveedor y Dirección");
                await transaction.RollbackAsync();
                return Problem();
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Actualiza un proveedor", Description = "Se actualizan todos los campos de un proveedor. No incluye sus relaciones*")]
        [SwaggerResponse(200, "Proveedor actualizado", typeof(ProveedoresDto))]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(404, "Proveedor no encontrado")]
        [SwaggerResponse(500, "Ocurrió un error inesperado")]
        public async Task<IActionResult> ActualizarProveedor(Guid id, [FromBody] ProveedoresDto changueProveedor)
        {
            try
            {
                if (id != changueProveedor.Id)
                {
                    logger.Errores("El ID del proveedor no coincide.");
                    return BadRequest(new { provedor = "El ID del proveedor no coincide." });
                }

                var proveedorExistente = await mdtecnologiaContext.Proveedores.FindAsync(id);
                if (proveedorExistente == null)
                {
                    logger.Errores("Proveedor no encontrado");
                    return NotFound();
                }
                var result = await ValidarProveedor(changueProveedor);
                if (result != null)
                    return result;

                proveedorExistente.Nombre = changueProveedor.Nombre;
                proveedorExistente.Telefono = changueProveedor.Telefono;
                proveedorExistente.Correo = changueProveedor.Correo;

                await mdtecnologiaContext.SaveChangesAsync();
                return Ok(new { proveddor = "Proveedor actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Error al actualizar el proveedor");
                return Problem("Error al actualizar el proveedor");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Eliminar un proveedor", Description = "Se elimina un proveedor de la base de datos")]
        [SwaggerResponse(204, "Proveedor eliminado")]
        [SwaggerResponse(404, "Proveedor no encontrado")]
        public async Task<ActionResult> EliminarProveedor(Guid id)
        {
            await using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                var proveedor = await mdtecnologiaContext.Proveedores.FindAsync(id);
                if (proveedor == null)
                {
                    logger.Depuracion("EL Proveedor a eliminar No Existe");
                    return NotFound();
                }

                mdtecnologiaContext.Proveedores.Remove(proveedor);
                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                logger.Advertencia($"Un Proveedor con {proveedor.Id} y nombre {proveedor.Nombre} ha sido Eliminado");
                return Ok(new { message = "Proveedor Eliminado Con éxito" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.Excepciones(ex, "Error en el endpoint EliminarProveedor");
                return Problem("Error en el endpoint EliminarProveedor");
            }
        }

        [SwaggerIgnore]
        private async Task<Proveedor> CrearProveedor(ProveedoresDto newProveedor)
        {
            var proveedor = new Proveedor()
            {
                Id = newProveedor.Id ?? Guid.NewGuid(),
                Correo = newProveedor.Correo,
                Nombre = newProveedor.Nombre,
                Direccion = newProveedor.Direccion?.Id,
                Telefono = newProveedor.Telefono,
            };
            await mdtecnologiaContext.Proveedores.AddAsync(proveedor);
            logger.Informacion("Se ha creado un Proveedor");
            return proveedor;
        }

        [SwaggerIgnore]
        private async Task<ActionResult?> ValidarProveedor(ProveedoresDto proveedorDto)
        {
            if (string.IsNullOrWhiteSpace(proveedorDto.Nombre))
                return BadRequest(new { nombre = "El nombre no es válido" });

            if (proveedorDto.Correo != null && !ClientesController.ValidarCorreo(proveedorDto.Correo))
                return BadRequest(new { correo = "El correo no es válido" });
            if (proveedorDto.Correo != null && await mdtecnologiaContext.Proveedores.AnyAsync(c => c.Correo != null && c.Correo.ToLower().Equals(proveedorDto.Correo.ToLower())))
            {
                logger.Errores("Correo ya en uso");
                return BadRequest(new { correo = "Correo ya en uso" });
            }

            if (proveedorDto.Telefono != null && string.IsNullOrWhiteSpace(proveedorDto.Telefono))
            {
                logger.Errores("Teléfono invalido");
                return BadRequest(new { telefono = "Teléfono invalido" });
            }
            if (proveedorDto.Telefono != null && await mdtecnologiaContext.Proveedores.AnyAsync(t => t.Telefono != null && t.Telefono.Equals(proveedorDto.Telefono)))
            {
                logger.Errores("Teléfono ya en uso");
                return BadRequest(new { telefono = "Teléfono ya en uso" });
            }

            if (proveedorDto.Direccion?.Id != null && !await mdtecnologiaContext.Direcciones.AnyAsync(d => d.Id == proveedorDto.Direccion.Id))
            {
                logger.Errores("Dirección no registrada");
                return BadRequest(new { direccion = "Dirección no registrada" });
            }

            return null;
        }
    }
}