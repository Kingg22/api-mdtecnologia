using MD_Tech.Contexts;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            this.logsApi = new LogsApi(typeof(ClientesController));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Clientes>>> getClientes()
        {
            return Ok(await mdtecnologiaContext.Clientes.ToListAsync());
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddCliente([FromBody] ClienteDto newCliente)
        {
            try
            {
                var idUser = User.FindFirstValue(ClaimTypes.Name);
                if (idUser == null || idUser != newCliente.IdUsuario.ToString())
                {
                    logsApi.Depuracion(idUser);
                    logsApi.Depuracion($"Rechazado porque el cliente no es el mismo usuario, {newCliente.IdUsuario}");
                    return Unauthorized();
                }
                else
                {
                    var cliente = await CrearCliente(newCliente);
                    if (cliente != null)
                    {
                        logsApi.Informacion("se ha creado un nuevo cliente");
                        return Created(Url.Action("GetCliente", "Cliente", new { id = cliente.Id }, Request.Scheme), new
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
                        return BadRequest();

                    }
                }
            }
            catch (Exception ex)
            {
                logsApi.Excepciones(ex, "ha Ocurrido un Error en el Enpoint AddCliente");
                return StatusCode(500, $"Error al insertar el cliente: {ex.Message}");

            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> getCliente(Guid idCliente)
        {
            var resultado = await mdtecnologiaContext.Clientes.FindAsync(idCliente);
            if (resultado == null)
            {
                return NotFound();
            }
            return Ok(new { cliente = resultado });
        }

        [HttpPost("usuario")]
        public async void CrearClienteUsuario(ClienteUsuarioDto clienteUsuario)
        {
            using var transaction = await mdtecnologiaContext.Database.BeginTransactionAsync();
            // Registrar primero el usuario y luego relacionarlo al cliente
            // devuelve el cliente completo con el detalle de su usuarioDto

        }

        private async Task<Clientes> CrearCliente(ClienteDto newCliente)
        {
            var usuario = await mdtecnologiaContext.Usuarios.FindAsync(newCliente.IdUsuario);
            if (newCliente.Telefono != null && await mdtecnologiaContext.Clientes.Where(cliente => cliente.Telefono == newCliente.Telefono).AnyAsync())
            {
                return null;
            }
            if (await mdtecnologiaContext.Clientes.Where(cliente => cliente.Correo == newCliente.Correo).AnyAsync())
            {
                return null;
            }
            if (usuario.Cliente != null)
            {
                return null;
            }

            var cliente = new Clientes()
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
