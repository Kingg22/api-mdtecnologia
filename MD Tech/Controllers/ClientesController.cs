using MD_Tech.Context;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly MdtecnologiaContext mdtecnologiaContext;
        private readonly LogsApi logsApi;

        public ClientesController(MdtecnologiaContext mdtecnologiaContext)
        {
            this.mdtecnologiaContext = mdtecnologiaContext;
            logsApi = new LogsApi(GetType());
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
                .Select(c => new ClienteDto()
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Apellido = c.Apellido,
                    Correo = c.Correo,
                    Telefono = c.Telefono,
                    IdUsuario = c.Usuario,
                    Direcciones = c.Direcciones.Select(d => new DireccionDto()
                    {
                        Id = d.Id,
                        Descripcion = d.Descripcion,
                        CreatedAt = d.CreatedAt,
                        Provincia = d.Provincia,
                    }).ToList()
                })
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
            return resultado != null ? Ok(new
            {
                cliente = new ClienteDto()
                {
                    Id = resultado.Id,
                    Nombre = resultado.Nombre,
                    Apellido = resultado.Apellido,
                    Correo = resultado.Correo,
                    Telefono = resultado.Telefono,
                    IdUsuario = resultado.Usuario
                }
            }) : NotFound();
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Crea un cliente", Description = "Agrega un nuevo cliente a la base de datos")]
        [SwaggerResponse(201, "Cliente creado", typeof(ClienteDto))]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(500, "Ha ocurrido un error inesperado")]
        public async Task<ActionResult> AddCliente([FromBody] ClienteDto newCliente)
        {
            try
            {
                var vali = await mdtecnologiaContext.Clientes.AnyAsync(n => n.Usuario == newCliente.IdUsuario);
                if (vali)
                {
                    logsApi.Errores("El Usuario que ingreso ya esta Afiliado a un cliente");
                    return BadRequest(new { Usuario = "Error, el Usuario que ingreso ya esta Afiliado a un cliente" });
                }
                var verificorreo = await mdtecnologiaContext.Clientes.AnyAsync(c => c.Correo == newCliente.Correo);
                if (verificorreo)
                {
                    logsApi.Informacion("El correo ya esta registrado");
                    return BadRequest(new { correo = "Correo ya en Uso" });
                }
                if (!newCliente.Correo.Contains('@') || newCliente.Correo.Count(c => c == '@') > 1 || string.IsNullOrWhiteSpace(newCliente.Correo))
                {
                    logsApi.Errores("El correo ingresado no cuenta con formato de correo");
                    return BadRequest(new { correo = "Ingrese un correo Valido" });
                }

                var cliente = await CrearCliente(newCliente);
                if (cliente != null)
                {
                    logsApi.Informacion("se ha creado un nuevo cliente");
                    return Created(Url.Action(nameof(GetClientes), "Cliente", new { id = cliente.Id }, Request.Scheme), new
                    {
                        cliente = new ClienteDto()
                        {
                            Nombre = cliente.Nombre,
                            Apellido = cliente.Apellido,
                            IdUsuario = cliente.Usuario,
                            Correo = cliente.Correo,
                            Id = cliente.Id,
                            Telefono = cliente.Telefono,
                        }
                    });
                }
                else
                {
                    logsApi.Depuracion("Error al crear el Usuario");
                    return BadRequest(new { dto = "revise los campos, pueden ya estar registrados" });
                }
            }
            catch (Exception ex)
            {
                logsApi.Excepciones(ex, "ha Ocurrido un Error en el Enpoint AddCliente");
                return Problem();

            }
        }

        [HttpPost("usuario")]
        public async Task<ActionResult> CrearClienteUsuario([FromBody] ClienteUsuarioDto clienteUsuario)
        {
            using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrWhiteSpace(clienteUsuario.Password) || clienteUsuario.Password.Contains(clienteUsuario.Username))
                {
                    logsApi.Informacion("registro rechazado por contraseña contiene usuario");
                    return BadRequest(new { password = "ingrese una contraseña válida" });
                }
                if (string.IsNullOrWhiteSpace(clienteUsuario.Username))
                {
                    logsApi.Informacion("No proporcionó un username");
                    return BadRequest(new { username = "el username es requerido" });
                }
                if (await mdtecnologiaContext.Usuarios.AnyAsync(u => u.Username.Equals(clienteUsuario.Username)))
                {
                    logsApi.Informacion("registro rechazado ya existe ese username");
                    return BadRequest(new { username = "nombre de usuario en uso, intente nuevamente" });
                }
                if (!clienteUsuario.Correo.Contains("@") || clienteUsuario.Correo.Count(c => c == '@') > 1 || string.IsNullOrWhiteSpace(clienteUsuario.Correo))
                {
                    logsApi.Errores("El correo ingresado no cuenta con formato de correo");
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
                var result = await mdtecnologiaContext.SaveChangesAsync();
                logsApi.Informacion("Se ha creado un nuevo Usuario");

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

                return Created(Url.Action(nameof(GetClientes), "Cliente", new { id = crearC.Id }, Request.Scheme),
                    new
                    {
                        cliente = new ClienteDto
                        {
                            Id = crearC.Id,
                            Nombre = crearC.Nombre,
                            Apellido = crearC.Apellido,
                            Correo = crearC.Correo,
                            Telefono = crearC.Telefono,
                            IdUsuario = crearC.Usuario,
                        }
                    });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logsApi.Excepciones(ex, "Ocurrió un error al crear el usuario y cliente");
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
                if (string.IsNullOrWhiteSpace(newCorreo.Correo) || !newCorreo.Correo.Contains("@"))
                {
                    logsApi.Informacion("Correo rechazado por no contener @");
                    return BadRequest(new { message = "Ingrese un correo válido" });
                }   
                var cliente = await mdtecnologiaContext.Clientes.FindAsync(newCorreo.Id);
                if (cliente == null)
                {
                    logsApi.Informacion($"Cliente con ID {newCorreo.Id} no encontrado para actualizar su correo");
                    return NotFound();
                }
                if (cliente.Correo == newCorreo.Correo) {
                    logsApi.Informacion("Correo rechazado por ser igual al actual");
                    return BadRequest(new { correo = "El correo proporcionado es igual al correo actual" });
                }
                var verificorreo = await mdtecnologiaContext.Clientes.AnyAsync(c => c.Correo == newCorreo.Correo);
                if (verificorreo)
                {
                    logsApi.Informacion("El correo ya esta registrado");
                    return BadRequest(new { correo = "Correo ya en Uso" });
                }

                cliente.Correo = newCorreo.Correo;
                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                logsApi.Informacion($"Correo del cliente con ID {cliente.Id} ha actualizado su correo");
                return Ok(new { message = "Correo actualizado con éxito" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logsApi.Excepciones(ex, $"Error al actualizar el correo del cliente con ID {newCorreo.Id}");
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
                    logsApi.Informacion($"Cliente no Encontrado para eliminar con id {id}");
                    return NotFound();
                }
                mdtecnologiaContext.Clientes.Remove(cliente);
                await mdtecnologiaContext.SaveChangesAsync();
                await transaction.CommitAsync();
                logsApi.Advertencia($"Cliente con ID {id} eliminado con éxito");
                return Ok(new { message = "Cliente eliminado con éxito" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logsApi.Excepciones(ex, $"Error al eliminar el cliente con ID {id}");
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
            {
                return null;
            }

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
