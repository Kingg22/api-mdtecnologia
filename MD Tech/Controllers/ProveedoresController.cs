using MD_Tech.Contexts;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly MdtecnologiaContext mdtecnologiaContext;
        private readonly LogsApi logApi;

        public ProveedoresController(MdtecnologiaContext mdtecnologiaContext)
        {
            this.mdtecnologiaContext = mdtecnologiaContext;
            logApi = new LogsApi(typeof(ProveedoresController)); ;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<ProveedoresDto>>> GetProveedor()
        {
            return Ok(new
            {
                proveedores = await mdtecnologiaContext.Proveedores
                .OrderBy(p => p.Nombre)
                .AsNoTracking()
                .Select(p => new ProveedoresDto()
                {
                    Id = p.Id,
                    Telefono = p.Telefono,
                    Correo = p.Correo,
                    Direccion = new DireccionDto() { Id = p.Direccion },
                    Nombre = p.Nombre,
                })
                .ToListAsync()
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ProveedoresDto>> GetProveedor(Guid id)
        {
            var provedor = await mdtecnologiaContext.Proveedores.FindAsync(id);
            return provedor == null ? NotFound() : Ok(new
            {
                proveedor = new ProveedoresDto()
                {
                    Id = provedor.Id,
                    Nombre = provedor.Nombre,
                    Correo = provedor.Correo,
                    Telefono = provedor.Telefono,
                    Direccion = new DireccionDto() { Id = provedor.Direccion }
                }
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProveedoresDto>> AgregarProvedor([FromBody] ProveedoresDto proveedorDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(proveedorDto.Nombre))
                {
                    return BadRequest(new { nombre = "El nombre no puede ser null" });
                }
                if (proveedorDto.Correo != null)
                {
                    var validar = await mdtecnologiaContext.Proveedores.AnyAsync(c => c.Correo != null && c.Correo.ToLower().Equals(proveedorDto.Correo));
                    if (validar)
                    {
                        logApi.Errores("Correo ya en uso");
                        return BadRequest(new { correo = "correo ya en uso" });
                    }
                }
                if (proveedorDto.Telefono != null)
                {
                    var result = await mdtecnologiaContext.Proveedores.AnyAsync(t => t.Telefono != null && t.Telefono.Equals(proveedorDto.Telefono));
                    if (result)
                    {
                        logApi.Errores("Telefono ya en uso");
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
                            logApi.Errores("Direccion No Registrada");
                            return BadRequest(new { direccion = "Direccion no registrada" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { direccion_Id = "No ingresó un Id de dirección" });
                    }
                }

                var proveedor = await CrearProveedor(proveedorDto);
                return Created(Url.Action(nameof(GetProveedor), "Proveedores", new { id = proveedor.Id }, Request.Scheme),
                   new
                   {
                       proveedor = new ProveedoresDto
                       {
                           Id = proveedor.Id,
                           Nombre = proveedor.Nombre,
                           Correo = proveedor.Correo,
                           Telefono = proveedor.Telefono,
                           Direccion = proveedor.Direccion == null ? null : new DireccionDto() { Id = proveedor.Direccion }
                       }
                   });
            }
            catch (Exception ex)
            {
                logApi.Excepciones(ex, "Error al Crear un provedor");
                return Problem();
            }
        }

        [HttpPost("direccion")]
        [Authorize]
        public async Task<ActionResult<ProveedoresDto>> CrearProvedorDireccion([FromBody] ProveedoresDto proveedorDto)
        {
            var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            Direcciones? direccion = null;
            try
            {
                if (string.IsNullOrWhiteSpace(proveedorDto.Nombre))
                {
                    return BadRequest(new { nombre = "El nombre no puede ser null" });
                }
                if (proveedorDto.Correo != null)
                {
                    var validar = await mdtecnologiaContext.Proveedores.AnyAsync(c => c.Correo != null && c.Correo.ToLower().Equals(proveedorDto.Correo));
                    if (validar)
                    {
                        logApi.Errores("Correo ya en uso");
                        return BadRequest(new { correo = "correo ya en uso" });
                    }
                }
                if (proveedorDto.Telefono != null)
                {
                    var result = await mdtecnologiaContext.Proveedores.AnyAsync(t => t.Telefono != null && t.Telefono.Equals(proveedorDto.Telefono));
                    if (result)
                    {
                        logApi.Errores("Telefono ya en uso");
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
                            logApi.Errores("Direccion Ya Registrada");
                            return BadRequest(new { direccion_Id = "Id en uso, si no desea crear una dirección nueva utilizar otro endpoint" });
                        }
                    }
                    if (proveedorDto.Direccion.Provincia == null)
                    {
                        return BadRequest(new { direccion_Provincia = "la provincia es requerida para crear una dirección" });
                    }
                    if (!await mdtecnologiaContext.Provincias.AnyAsync(p => p.Id == proveedorDto.Direccion.Provincia))
                    {
                        return BadRequest(new { direccion_Provincia = "provincia no es válida, intente nuevamente" });
                    }
                    direccion = new Direcciones()
                    {
                        Id = proveedorDto.Direccion.Id ?? Guid.NewGuid(),
                        Provincia = (int)proveedorDto.Direccion.Provincia,
                        Descripcion = proveedorDto.Direccion.Descripcion,
                    };
                    await mdtecnologiaContext.Direcciones.AddAsync(direccion);
                    await mdtecnologiaContext.SaveChangesAsync();
                    logApi.Informacion("Se ha creado una Direccion");
                }
                // Se actualiza la referencia de direcciones
                proveedorDto.Direccion = new DireccionDto() { Id = direccion?.Id };

                var proveedor = await CrearProveedor(proveedorDto);
                await transaction.CommitAsync();
                return Created(Url.Action(nameof(GetProveedor), "provedor", new { id = proveedor.Id }, Request.Scheme), new
                {
                    proveedor = new ProveedoresDto()
                    {
                        Id = proveedor.Id,
                        Correo = proveedor.Correo,
                        Nombre = proveedor.Nombre,
                        Direccion = proveedor.Direccion == null ? null : new DireccionDto()
                        {
                            Id = proveedor.Direccion,
                            Descripcion = proveedor.DireccionNavigation?.Descripcion,
                            Provincia = proveedor.DireccionNavigation?.Provincia,
                            CreatedAt = proveedor.DireccionNavigation?.CreatedAt,
                        },
                        Telefono = proveedor.Telefono,
                    }
                });
            }
            catch (Exception ex)
            {
                logApi.Excepciones(ex, "Error al crear Provedor y Direccion");
                await transaction.RollbackAsync();
                return Problem();
            }
        }

        [SwaggerIgnore]
        private async Task<Proveedores> CrearProveedor([FromBody] ProveedoresDto newProvedor)
        {
            var provedor = new Proveedores()
            {
                Correo = newProvedor.Correo,
                Nombre = newProvedor.Nombre,
                Direccion = newProvedor.Direccion?.Id,
                Telefono = newProvedor.Telefono,
            };
            await mdtecnologiaContext.Proveedores.AddAsync(provedor);
            await mdtecnologiaContext.SaveChangesAsync();
            logApi.Informacion("Se ha creado un Proveedor");
            return provedor;
        }
    }
}
