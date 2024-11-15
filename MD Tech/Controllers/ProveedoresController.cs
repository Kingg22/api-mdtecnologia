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
                .OrderBy(p => p.Nombre)
                .AsNoTracking()
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
        public async Task<ActionResult<ProveedoresDto>> AgregarProvedor([FromBody] ProveedoresDto proveedorDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(proveedorDto.Nombre))
                    return BadRequest(new { nombre = "El nombre no puede ser null" });
                if (proveedorDto.Correo != null)
                {
                    if (await mdtecnologiaContext.Proveedores.AnyAsync(c => c.Correo != null && c.Correo.ToLower().Equals(proveedorDto.Correo)))
                    {
                        logger.Errores("Correo ya en uso");
                        return BadRequest(new { correo = "correo ya en uso" });
                    }
                }
                if (proveedorDto.Telefono != null)
                {
                    var result = await mdtecnologiaContext.Proveedores.AnyAsync(t => t.Telefono != null && t.Telefono.Equals(proveedorDto.Telefono));
                    if (result)
                    {
                        logger.Errores("Telefono ya en uso");
                        return BadRequest(new { telefono = "teléfono ya en uso" });
                    }
                }
                if (proveedorDto.Direccion != null)
                {
                    if (proveedorDto.Direccion.Id != null)
                    {
                        var validarDireccio = await mdtecnologiaContext.Direcciones.FindAsync(proveedorDto.Direccion.Id);
                        if (validarDireccio == null)
                        {
                            logger.Errores("Direccion No Registrada");
                            return BadRequest(new { direccion = "Direccion no registrada" });
                        }
                    }
                    else
                        return BadRequest(new { direccion_Id = "No ingresó un Id de dirección" });
                }

                var proveedor = await CrearProveedor(proveedorDto);
                return Created(Url.Action(nameof(GetProveedor), "Proveedores", new { id = proveedor.Id }, Request.Scheme), new { proveedor = new ProveedoresDto(proveedor), });
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Error al Crear un provedor");
                return Problem();
            }
        }

        [HttpPost("direccion")]
        [Authorize]
        [SwaggerOperation(Summary = "Crea un proveedor con su dirección", Description = "Agrega un nuevo proveedor a la base de datos")]
        [SwaggerResponse(201, "Proveedor creado", typeof(ProveedoresDto))]
        [SwaggerResponseHeader(201, "location", "string", "Enlace al recurso creado")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult<ProveedoresDto>> CrearProvedorDireccion([FromBody] ProveedoresDto proveedorDto)
        {
            var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            Direccion? direccion = null;
            try
            {
                if (string.IsNullOrWhiteSpace(proveedorDto.Nombre))
                    return BadRequest(new { nombre = "El nombre no puede ser null" });
                if (proveedorDto.Correo != null)
                {
                    if (await mdtecnologiaContext.Proveedores.AnyAsync(c => c.Correo != null && c.Correo.ToLower().Equals(proveedorDto.Correo)))
                    {
                        logger.Errores("Correo ya en uso");
                        return BadRequest(new { correo = "correo ya en uso" });
                    }
                }
                if (proveedorDto.Telefono != null)
                {
                    if (await mdtecnologiaContext.Proveedores.AnyAsync(t => t.Telefono != null && t.Telefono.Equals(proveedorDto.Telefono)))
                    {
                        logger.Errores("Telefono ya en uso");
                        return BadRequest(new { telefono = "teléfono ya en uso" });
                    }
                }
                if (proveedorDto.Direccion != null)
                {
                    if (proveedorDto.Direccion.Id != null)
                    {
                        var validarDireccio = await mdtecnologiaContext.Direcciones.FindAsync(proveedorDto.Direccion.Id);
                        if (validarDireccio != null)
                        {
                            logger.Errores("Direccion Ya Registrada");
                            return BadRequest(new { direccion_Id = "Id en uso, si no desea crear una dirección nueva utilizar otro endpoint" });
                        }
                    }
                    if (proveedorDto.Direccion.Provincia == null)
                        return BadRequest(new { direccion_Provincia = "la provincia es requerida para crear una dirección" });
                    if (!await mdtecnologiaContext.Provincias.AnyAsync(p => p.Id == proveedorDto.Direccion.Provincia))
                        return BadRequest(new { direccion_Provincia = "provincia no es válida, intente nuevamente" });
                    direccion = new Direccion()
                    {
                        Id = proveedorDto.Direccion.Id ?? Guid.NewGuid(),
                        Provincia = (int)proveedorDto.Direccion.Provincia,
                        Descripcion = proveedorDto.Direccion.Descripcion,
                    };
                    await mdtecnologiaContext.Direcciones.AddAsync(direccion);
                    await mdtecnologiaContext.SaveChangesAsync();
                    logger.Informacion("Se ha creado una Direccion");
                }
                // Se actualiza la referencia de direcciones
                proveedorDto.Direccion = new DireccionDto() { Id = direccion?.Id };

                var proveedor = await CrearProveedor(proveedorDto);
                await transaction.CommitAsync();
                return Created(Url.Action(nameof(GetProveedor), "provedor", new { id = proveedor.Id }, Request.Scheme), new { proveedor = new ProveedoresDto(proveedor) });
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Error al crear Provedor y Direccion");
                await transaction.RollbackAsync();
                return Problem();
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Actualiza un proveedor", Description = "Se actualizan todos los campos de un proveedor. No incluye sus relaciones*")]
        [SwaggerResponse(200, "Proveedor actualizado", typeof(UsuarioDto))]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(404, "Usuario no encontrado")]
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
                    logger.Errores("Proveeddor no encontrado");
                    return NotFound(new { provedor = "Proveedor no encontrado." });
                }
                if (string.IsNullOrWhiteSpace(changueProveedor.Nombre))
                {
                    logger.Errores("El nombre es invadilo");
                    return BadRequest(new { nombre = "Nombre invalido" });
                }
                // Campos opcionales
                if (changueProveedor.Telefono != null && string.IsNullOrWhiteSpace(changueProveedor.Telefono))
                {
                    logger.Errores("Telefono invalido");
                    return BadRequest(new { telefono = "Telefono invalido" });
                }
                if (changueProveedor.Telefono != null && await mdtecnologiaContext.Proveedores.AnyAsync(t => t.Telefono != null && t.Telefono == changueProveedor.Telefono))
                {
                    logger.Errores("El Telefono ya esta en uso");
                    return BadRequest(new { Telefono = "El telefono ya en uso" });
                }
                if (changueProveedor.Correo != null)
                {
                    if (string.IsNullOrWhiteSpace(changueProveedor.Correo) || !changueProveedor.Correo.Contains('@') || changueProveedor.Correo.Count(c => c == '@') > 1)
                    {
                        logger.Errores("Correo Con formato invalido");
                        return BadRequest(new { correo = "El correo no tiene el formato adecuado" });
                    }
                    if (await mdtecnologiaContext.Proveedores.AnyAsync(p => p.Id != changueProveedor.Id && p.Correo != null && p.Correo.ToLower().Equals(changueProveedor.Correo)))
                    {
                        logger.Errores("El correo que se ingreso, se encuentra en uso");
                        return BadRequest(new { correo = "El correo ya esta en uso" });
                    }
                }

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
            using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
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
        private async Task<Proveedor> CrearProveedor(ProveedoresDto newProvedor)
        {
            var provedor = new Proveedor()
            {
                Correo = newProvedor.Correo,
                Nombre = newProvedor.Nombre,
                Direccion = newProvedor.Direccion?.Id,
                Telefono = newProvedor.Telefono,
            };
            await mdtecnologiaContext.Proveedores.AddAsync(provedor);
            await mdtecnologiaContext.SaveChangesAsync();
            logger.Informacion("Se ha creado un Proveedor");
            return provedor;
        }
    }
}
