using MD_Tech.Context;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Caching;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

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
            using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                if (await mdtecnologiaContext.Clientes.AnyAsync(n => n.Usuario == newCliente.IdUsuario))
                {
                    logger.Errores("El Usuario que ingreso ya está afiliado a un cliente");
                    return BadRequest(new { Usuario = "Error, el Usuario que ingresó ya está afiliado a un cliente" });
                }
                if (await mdtecnologiaContext.Clientes.AnyAsync(c => c.Correo == newCliente.Correo))
                {
                    logger.Informacion("El correo ya está registrado");
                    return BadRequest(new { correo = "Correo ya en uso" });
                }
                if (!newCliente.Correo.Contains('@') || newCliente.Correo.Count(c => c == '@') > 1 || string.IsNullOrWhiteSpace(newCliente.Correo))
                {
                    logger.Errores("El correo ingresado no cuenta con formato válido");
                    return BadRequest(new { correo = "Ingrese un correo válido" });
                }
                
                var cliente = await CrearCliente(newCliente);
                if (cliente == null)
                {
                    logger.Errores("Error al crear el cliente");
                    return BadRequest(new { dto = "Revise los campos, pueden ya estar registrados" });
                }
                if (newCliente.Direcciones != null)
                {
                    foreach (var direccionDto in newCliente.Direcciones)
                    {
                        if (direccionDto.Provincia < 0 || direccionDto.Provincia > 10)
                        {
                            logger.Errores($"Provincia: {direccionDto.Provincia} inválida");
                            await transaction.RollbackAsync();
                            return BadRequest(new { Provincia = $"La provincia {direccionDto.Provincia} es inválida. Las provincias van del 1 al 10" });
                        }
                        if(!await mdtecnologiaContext.Provincias.AnyAsync(p=> p.Id==direccionDto.Provincia) || direccionDto.Provincia ==null)
                        {
                            logger.Errores($"Provincia: {direccionDto.Provincia} inválida");
                            await transaction.RollbackAsync();
                            return BadRequest(new { Provincia = $"La provincia {direccionDto.Provincia} No se encuentra Registrada" });
                        }
                        if(string.IsNullOrWhiteSpace(direccionDto.Descripcion))
                        {
                            logger.Errores("La Descripcion Es invalida");
                            await transaction.RollbackAsync();
                            return BadRequest(new { Descripcion = $"La Descripcion Es invalida" });
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
                            Cliente = cliente.Id,
                            Direccion = direccion.Id
                        }; 
                        await mdtecnologiaContext.DireccionesClientes.AddAsync(enlace);
                    }
                    await mdtecnologiaContext.SaveChangesAsync();
                }
                await transaction.CommitAsync();
                await mdtecnologiaContext.Entry(cliente).ReloadAsync();
                await mdtecnologiaContext.Entry(cliente).Collection(d => d.DireccionClientes).LoadAsync();
                await mdtecnologiaContext.Entry(cliente).Collection(di => di.Direcciones).LoadAsync();

                logger.Informacion("Se ha creado un nuevo cliente");
                return Created(Url.Action(nameof(GetClientes), "Cliente", new { id = cliente.Id }, Request.Scheme), new { cliente = new ClienteDto(cliente) });
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
        public async Task<ActionResult> CrearClienteUsuario([FromBody] ClienteUsuarioDto clienteUsuario)
        {
            using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(clienteUsuario.Password) || clienteUsuario.Password.Contains(clienteUsuario.Username))
                {
                    logger.Informacion("registro rechazado por contraseña contiene usuario");
                    return BadRequest(new { password = "ingrese una contraseña válida" });
                }
                if (string.IsNullOrWhiteSpace(clienteUsuario.Username))
                {
                    logger.Informacion("No proporcionó un username");
                    return BadRequest(new { username = "el username es requerido" });
                }
                if (await mdtecnologiaContext.Usuarios.AnyAsync(u => u.Username.Equals(clienteUsuario.Username)))
                {
                    logger.Informacion("registro rechazado ya existe ese username");
                    return BadRequest(new { username = "nombre de usuario en uso, intente nuevamente" });
                }
                if (!clienteUsuario.Correo.Contains("@") || clienteUsuario.Correo.Count(c => c == '@') > 1 || string.IsNullOrWhiteSpace(clienteUsuario.Correo))
                {
                    logger.Errores("El correo ingresado no cuenta con formato de correo");
                    return BadRequest(new { correo = "Ingrese un correo Valido" });
                }
                var usuario = new Usuario
                {
                    Id = clienteUsuario.UsuarioID != null ? (Guid)clienteUsuario.UsuarioID : Guid.NewGuid(),
                    Password = BCrypt.Net.BCrypt.HashPassword(clienteUsuario.Password),
                    Username = clienteUsuario.Username,
                    Disabled = clienteUsuario.Disabled,
                    Rol = RolesEnum.cliente.ToString()
                };
                await mdtecnologiaContext.Usuarios.AddAsync(usuario);
                await mdtecnologiaContext.SaveChangesAsync();
                logger.Informacion("Se ha creado un nuevo Usuario");

                var clientedto = new ClienteDto
                {
                    Id = clienteUsuario.ClienteID != null ? clienteUsuario.ClienteID : Guid.NewGuid(),
                    Nombre = clienteUsuario.Nombre,
                    Apellido = clienteUsuario.Apellido,
                    Correo = clienteUsuario.Correo,
                    Telefono = clienteUsuario.Telefono,
                    IdUsuario = usuario.Id,
                };
                var crearC = await CrearCliente(clientedto);
                if (crearC == null)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { message = "revise los campos, pueden ya estar registrados" });
                }
                await transaction.CommitAsync();

                return Created(Url.Action(nameof(GetClientes), "Cliente", new { id = crearC.Id }, Request.Scheme), new { cliente = new ClienteDto(crearC) });
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
            using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            try
            {
                if (id != newCorreo.Id)
                {
                    logger.Informacion($"Cliente con id {id} no coincide con el body id {newCorreo.Id}");
                    return BadRequest(new { id = "El id del body no coincide con la ruta" });
                }
                if (string.IsNullOrWhiteSpace(newCorreo.Correo) || !newCorreo.Correo.Contains('@'))
                {
                    logger.Informacion("Correo rechazado por no contener @");
                    return BadRequest(new { correo = "Ingrese un correo válido" });
                }
                var cliente = await mdtecnologiaContext.Clientes.FindAsync(newCorreo.Id);
                if (cliente == null)
                {
                    logger.Informacion($"Cliente con ID {newCorreo.Id} no encontrado para actualizar su correo");
                    return NotFound();
                }
                if (cliente.Correo == newCorreo.Correo)
                {
                    logger.Informacion("Correo rechazado por ser igual al actual");
                    return BadRequest(new { correo = "El correo proporcionado es igual al correo actual" });
                }
                var verificorreo = await mdtecnologiaContext.Clientes.AnyAsync(c => c.Correo == newCorreo.Correo);
                if (verificorreo)
                {
                    logger.Informacion("El correo ya esta registrado");
                    return BadRequest(new { correo = "Correo ya en Uso" });
                }

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
            using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
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
                (newCliente.Telefono != null && await mdtecnologiaContext.Clientes.AnyAsync(cliente => cliente.Telefono == newCliente.Telefono)) ||
                await mdtecnologiaContext.Clientes.AnyAsync(cliente => cliente.Correo == newCliente.Correo) ||
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
    }
}
