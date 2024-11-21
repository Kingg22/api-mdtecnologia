using MD_Tech.Context;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net.Mail;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly MdtecnologiaContext mdtecnologiaContext;
        private readonly LogsApi<ClientesController> logger;

        public ClientesController(MdtecnologiaContext mdtecnologiaContext, LogsApi<ClientesController> logger)
        {
            this.mdtecnologiaContext = mdtecnologiaContext;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene todos los clientes", Description = "Devuelve una lista de clientes")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<ClienteDto>))]
        public async Task<ActionResult<List<Cliente>>> GetClientes()
        {
            return Ok(new
            {
                clientes = await mdtecnologiaContext.Clientes
                .AsNoTracking()
                .Include(c => c.Direcciones)
                .Select(c => new ClienteDto(c))
                .ToListAsync()
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene un cliente por ID", Description = "Devuelve el detalle del cliente")]
        [SwaggerResponse(200, "Operación exitosa", typeof(ClienteDto))]
        [SwaggerResponse(404, "Producto no encontrado")]
        public async Task<ActionResult> GetClientes(Guid id)
        {
            var resultado = await mdtecnologiaContext.Clientes.FindAsync(id);
            return resultado == null ? NotFound() : Ok(new { cliente = new ClienteDto(resultado) });
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Crea un cliente", Description = "Agrega un nuevo cliente a la base de datos")]
        [SwaggerResponse(201, "Cliente creado", typeof(ClienteDto))]
        [SwaggerResponseHeader(201, "location", "string", "Enlace al recurso creado")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult> AddCliente([FromBody] ClienteDto newCliente)
        {
            await using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                if (newCliente.IdUsuario != null && await mdtecnologiaContext.Clientes.AnyAsync(n => n.Usuario != null && n.Usuario == newCliente.IdUsuario))
                {
                    logger.Errores("El Usuario que ingreso ya está afiliado a un cliente");
                    return BadRequest(new { Usuario = "Error, el Usuario que ingresó ya está afiliado a un cliente" });
                }
                var result = await ValidarCorreoAsync(newCliente.Correo);
                if (result != null)
                    return result;

                var cliente = await CrearCliente(newCliente);
                if (cliente == null)
                {
                    logger.Errores("Error al crear el cliente");
                    return BadRequest(new { message = "Revise los campos, pueden ya estar registrados" });
                }
                if (newCliente.Direcciones.Count > 0)
                {
                    var resultado = await GuardarDirecciones(newCliente.Direcciones, cliente.Id);
                    if (resultado != null)
                        return resultado;
                }

                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await mdtecnologiaContext.Entry(cliente).ReloadAsync();
                await mdtecnologiaContext.Entry(cliente).Collection(c => c.Direcciones).LoadAsync();

                logger.Informacion("Se ha creado un nuevo cliente");
                return Created(Url.Action(nameof(GetClientes), "Clientes", new { id = cliente.Id }, Request.Scheme), new { cliente = new ClienteDto(cliente) });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.Excepciones(ex, "Ha ocurrido un error en el endpoint AddCliente");
                return Problem();
            }
        }

        [HttpPost("usuario")]
        [SwaggerOperation(Summary = "Crea un cliente con su usuario", Description = "Agrega un nuevo cliente y usuario a la base de datos")]
        [SwaggerResponse(201, "Cliente creado", typeof(ClienteDto))]
        [SwaggerResponseHeader(201, "location", "string", "Enlace al recurso creado")]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult> CrearClienteUsuario([FromBody] ClienteUsuarioDto newCliente)
        {
            await using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(newCliente.Password) || newCliente.Password.Contains(newCliente.Username))
                {
                    logger.Errores("registro rechazado por contraseña contiene usuario");
                    return BadRequest(new { password = "ingrese una contraseña válida" });
                }
                if (string.IsNullOrWhiteSpace(newCliente.Username))
                {
                    logger.Errores("No proporcionó un username");
                    return BadRequest(new { username = "el username es requerido" });
                }
                if (await mdtecnologiaContext.Usuarios.AnyAsync(u => u.Username.Equals(newCliente.Username)))
                {
                    logger.Errores("registro rechazado ya existe ese username");
                    return BadRequest(new { username = "username en uso, intente nuevamente" });
                }
                var result = await ValidarCorreoAsync(newCliente.Correo);
                if (result != null)
                    return result;

                var usuario = new Usuario
                {
                    Id = newCliente.UsuarioId ?? Guid.NewGuid(),
                    Password = BCrypt.Net.BCrypt.HashPassword(newCliente.Password),
                    Username = newCliente.Username,
                    Disabled = newCliente.Disabled,
                    Rol = RolesEnum.cliente.ToString()
                };
                await mdtecnologiaContext.Usuarios.AddAsync(usuario);
                logger.Informacion("Se ha creado un nuevo Usuario");

                var clientedto = new ClienteDto
                {
                    Id = newCliente.ClienteId ?? Guid.NewGuid(),
                    Nombre = newCliente.Nombre,
                    Apellido = newCliente.Apellido,
                    Correo = newCliente.Correo,
                    Telefono = newCliente.Telefono,
                    IdUsuario = usuario.Id,
                };
                var cliente = await CrearCliente(clientedto);
                if (cliente == null)
                {
                    logger.Errores("Error al crear cliente, eliminando su usuario");
                    return BadRequest(new { message = "revise los campos, pueden ya estar registrados" });
                }
                if (newCliente.Direcciones.Count > 0)
                {
                    var resultado = await GuardarDirecciones(newCliente.Direcciones, cliente.Id);
                    if (resultado != null)
                        return resultado;
                }
                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await mdtecnologiaContext.Entry(cliente).ReloadAsync();
                await mdtecnologiaContext.Entry(cliente).Collection(di => di.Direcciones).LoadAsync();
                return Created(Url.Action(nameof(GetClientes), "Clientes", new { id = cliente.Id }, Request.Scheme), new { cliente = new ClienteDto(cliente) });
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Ocurrió un error al crear el usuario y cliente");
                await transaction.RollbackAsync();
                return Problem();
            }
        }

        [HttpPatch("correo/{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Cambiar correo electrónico", Description = "Se actualizan el correo del cliente en la base de datos")]
        [SwaggerResponse(200, "Cliente actualizado", typeof(ClienteDto))]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(404, "Cliente no encontrado")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult> CambiarCorreo(Guid id, [FromBody] EmailChagueDto newCorreo)
        {
            await using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                if (id != newCorreo.Id)
                {
                    logger.Errores($"Cliente con id {id} no coincide con el body id {newCorreo.Id}");
                    return BadRequest(new { id = "El id del body no coincide con la ruta" });
                }
                var cliente = await mdtecnologiaContext.Clientes.FindAsync(newCorreo.Id);
                if (cliente == null)
                {
                    logger.Errores($"Cliente con ID {newCorreo.Id} no encontrado para actualizar su correo");
                    return NotFound();
                }
                if (cliente.Correo.Equals(newCorreo.Correo, StringComparison.OrdinalIgnoreCase))
                {
                    logger.Errores("Correo rechazado por ser igual al actual");
                    return BadRequest(new { correo = "El correo proporcionado es igual al correo actual" });
                }
                var result = await ValidarCorreoAsync(newCorreo.Correo);
                if (result != null)
                    return result;

                cliente.Correo = newCorreo.Correo;
                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                logger.Informacion($"Correo del cliente con ID {cliente.Id} ha actualizado su correo");
                return Ok(new { message = "Correo actualizado con éxito" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.Excepciones(ex, $"Error al actualizar el correo del cliente con ID {newCorreo.Id}");
                return Problem();
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Elimina un cliente", Description = "Se elimina un cliente de la base de datos")]
        [SwaggerResponse(204, "Cliente eliminado")]
        [SwaggerResponse(404, "Cliente no encontrado")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult> DeleteCliente(Guid id)
        {
            await using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                var cliente = await mdtecnologiaContext.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    logger.Informacion($"Cliente no Encontrado para eliminar con id {id}");
                    return NotFound();
                }
                mdtecnologiaContext.Clientes.Remove(cliente);
                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                logger.Advertencia($"Cliente con ID {id} eliminado con éxito");
                return Ok(new { message = "Cliente eliminado con éxito" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.Excepciones(ex, $"Error al eliminar el cliente con ID {id}");
                return Problem("Ocurrió un error al eliminar el cliente.");
            }
        }

        [SwaggerIgnore]
        private async Task<Cliente?> CrearCliente(ClienteDto newCliente)
        {
            var usuario = await mdtecnologiaContext.Usuarios.FindAsync(newCliente.IdUsuario);
            if (usuario == null ||
                (newCliente.Telefono != null && await mdtecnologiaContext.Clientes.AnyAsync(cliente => cliente.Telefono != null && cliente.Telefono == newCliente.Telefono)) ||
                await mdtecnologiaContext.Clientes.AnyAsync(cliente => cliente.Correo.ToLower().Equals(newCliente.Correo.ToLower())) ||
                usuario.Cliente != null)
            { return null; }

            var cliente = new Cliente()
            {
                Nombre = newCliente.Nombre,
                Apellido = newCliente.Apellido,
                Correo = newCliente.Correo,
                Telefono = newCliente.Telefono,
                Usuario = usuario.Id
            };
            await mdtecnologiaContext.Clientes.AddAsync(cliente);
            await mdtecnologiaContext.SaveChangesAsync();
            return cliente;
        }

        [SwaggerIgnore]
        public static bool ValidarCorreo(string correo)
        {
            try
            {
                _ = new MailAddress(correo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [SwaggerIgnore]
        private async Task<ActionResult?> ValidarCorreoAsync(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo) || !ValidarCorreo(correo))
            {
                logger.Errores("El correo ingresado no cuenta con formato válido");
                return BadRequest(new { correo = "Ingrese un correo válido" });
            }
            if (await mdtecnologiaContext.Clientes.AnyAsync(c => c.Correo.ToLower().Equals(correo.ToLower())))
            {
                logger.Errores("El correo ya esta registrado");
                return BadRequest(new { correo = "Correo ya en Uso" });
            }

            return null;
        }

        [SwaggerIgnore]
        private async Task<ActionResult?> GuardarDirecciones(ICollection<DireccionDto> direcciones, Guid idCliente)
        {
            foreach (var direccionDto in direcciones)
            {
                if (direccionDto.Provincia == null || !await mdtecnologiaContext.Provincias.AnyAsync(p => p.Id == direccionDto.Provincia))
                {
                    logger.Errores($"Provincia: {direccionDto.Provincia} inválida");
                    return BadRequest(new { Provincia = $"La provincia {direccionDto.Provincia} No se encuentra Registrada" });
                }
                if (string.IsNullOrWhiteSpace(direccionDto.Descripcion))
                {
                    logger.Errores("La Descripción de dirección Es invalida");
                    return BadRequest(new { Descripcion = $"La Descripción Es inválida" });
                }

                var direccion = new Direccion
                {
                    Id = Guid.NewGuid(),
                    Provincia = (int)direccionDto.Provincia,
                    Descripcion = direccionDto.Descripcion,
                };
                await mdtecnologiaContext.Direcciones.AddAsync(direccion);

                var enlace = new DireccionesCliente
                {
                    Cliente = idCliente,
                    Direccion = direccion.Id
                };
                await mdtecnologiaContext.DireccionesClientes.AddAsync(enlace);
            }

            return null;
        }
    }
}
