using MD_Tech.Context;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly MdtecnologiaContext Mdtecnologia;
        private readonly RsaSecurityKey PrivateKey;
        private readonly LogsApi<UsuariosController> logger;

        public UsuariosController(MdtecnologiaContext Mdtecnologia, LogsApi<UsuariosController> logger)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(System.IO.File.ReadAllText("private.pem"));
            PrivateKey = new RsaSecurityKey(rsa);
            this.Mdtecnologia = Mdtecnologia;
            this.logger = logger;
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Iniciar sesión", Description = "Concede acceso a los endpoints privados de la API")]
        [SwaggerResponse(200, "Acceso concedido", typeof(Dictionary<string, string>))]
        [SwaggerResponse(401, "Acceso denegado, razones no especificadas")]
        public async Task<ActionResult<Dictionary<string, string>>> Login([FromBody] LoginRequest loginRequest)
        {
            var usuario = await Mdtecnologia.Usuarios.FirstOrDefaultAsync(u => u.Username.Equals(loginRequest.Email));
            if (usuario != null)
            {
                if (usuario.Password != null)
                {
                    logger.Depuracion($"Usuario ID: {usuario.Id}");
                    var passMatch = BCrypt.Net.BCrypt.Verify(loginRequest.Password, usuario.Password);
                    logger.Depuracion($"Bcrypt verify: {passMatch}");
                    if (passMatch && !usuario.Disabled)
                    {
                        var tokenDes = new SecurityTokenDescriptor()
                        {
                            Subject = new ClaimsIdentity([
                                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), 
                                new Claim(ClaimTypes.Role, usuario.Rol),]),
                            Expires = DateTime.UtcNow.AddHours(1),
                            SigningCredentials = new SigningCredentials(PrivateKey, SecurityAlgorithms.RsaSha256),
                            IssuedAt = DateTime.UtcNow,
                            Issuer = "MDTech",
                            NotBefore = DateTime.UtcNow
                        };
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtToken = tokenHandler.CreateToken(tokenDes);
                        logger.Informacion($"Se ha generado un nuevo token al usuario: {usuario.Id}");
                        return Ok(new { token = tokenHandler.WriteToken(jwtToken) });
                    }
                    else
                        logger.Informacion("login fallido: la contraseña no coincide y/o usuario deshabilitado");
                }
                else
                    logger.Informacion("login fallido: el usuario no tiene contraseña");
            }
            else
                logger.Informacion("login fallido: no se encontró usuario en la BD");
            return Unauthorized();
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Crea un usuario", Description = "Agrega un nuevo usuario a la base de datos")]
        [SwaggerResponse(201, "Usuario creado", typeof(UsuarioDto))]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        public async Task<ActionResult<UsuarioDto>> CreateUser([FromBody] RegisterUserDto register)
        {
            if (await Mdtecnologia.Usuarios.AnyAsync(u => u.Username.Equals(register.Username)))
            {
                logger.Informacion("registro rechazado ya existe ese username");
                return BadRequest(new { username = "nombre de usuario en uso, intente nuevamente" });
            }
            if (string.IsNullOrWhiteSpace(register.Password) || register.Password.Contains(register.Username))
            {
                logger.Informacion("registro rechazado por contraseña contiene usuario");
                return BadRequest(new { password = "ingrese una contraseña válida" });
            }
            if (string.IsNullOrWhiteSpace(register.Username))
            {
                logger.Informacion("No proporcionó un username");
                return BadRequest(new { username = "el username es requerido" });
            }
            if (register.Id != null && await Mdtecnologia.Usuarios.FindAsync(register.Id) != null)
                return BadRequest(new { Id = "id proporcionado en uso" });
            var usuario = new Usuario()
            {
                Id = register.Id ?? Guid.NewGuid(),
                Username = register.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Rol = register.Rol.ToString(),
                Disabled = register.Disabled,
            };
            await Mdtecnologia.Usuarios.AddAsync(usuario);
            await Mdtecnologia.SaveChangesAsync();
            logger.Informacion($"nuevo usuario creado ID: {usuario.Id}");
            return Created(Url.Action(nameof(GetUsuario), "Usuarios", new { id = usuario.Id }, Request.Scheme), new
            {
                usuario = new UsuarioDto(usuario),
                warning = "debe relacionar el usuario"
            });
        }

        [HttpPatch("restore-password")]
        [SwaggerOperation(Summary = "Cambiar contraseña", Description = "Actualiza únicamente la contraseña del usuario")]
        [SwaggerResponse(200, "Contraseña actualizada", typeof(Dictionary<string, string>))]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(404, "Usuario no encontrado")]
        public async Task<ActionResult<Dictionary<string, string>>> ChangePassword([FromBody] RestoreDto restore)
        {
            var usuario = await Mdtecnologia.Usuarios.FirstOrDefaultAsync(u => u.Username.Equals(restore.Username));
            if (usuario == null)
                return NotFound();
            else
            {
                if (BCrypt.Net.BCrypt.Verify(restore.Password, usuario.Password))
                {
                    logger.Informacion("cambio de contraseña rechazado por ser igual a la actual");
                    return BadRequest(new { password = "la nueva contraseña no puede ser igual a la actual" });
                }
                if (restore.Password.Contains(usuario.Username, StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.Informacion("cambio de contraseña rechazado por contener su usuario");
                    return BadRequest(new { password = "la contraseña no puede tener su usuario" });
                }
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(restore.Password);
                await Mdtecnologia.SaveChangesAsync();
                logger.Informacion("nueva contraseña creada");
                return Ok(new { message = "se ha cambiado su contraseña de forma exitosa" });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Actualiza un usuario", Description = "Se actualizan todos los campos de un usuario. No incluye sus relaciones")]
        [SwaggerResponse(200, "Usuario actualizado", typeof(UsuarioDto))]
        [SwaggerResponse(400, "Datos de entrada inválidos")]
        [SwaggerResponse(404, "Usuario no encontrado")]
        public async Task<ActionResult<UsuarioDto>> UpdateProfile(Guid id, [FromBody] UsuarioUpdateDto usuarioDto)
        {
            if (id != usuarioDto.Id)
            {
                logger.Informacion($"usuario de id {id} no coincide con el body id {usuarioDto.Id}");
                return BadRequest();
            }
            if (usuarioDto.Password.Contains(usuarioDto.Username))
            {
                logger.Informacion("cambio de contraseña rechazado por contener su usuario");
                return BadRequest(new { password = "la contraseña no puede tener su usuario" });
            }
            var usuario = await Mdtecnologia.Usuarios.FindAsync(usuarioDto.Id);
            if (usuario == null)
                return NotFound();
            else
            {
                usuario.Username = usuarioDto.Username;
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password);
                usuario.Rol = usuarioDto.Rol.ToString();
                await Mdtecnologia.SaveChangesAsync();
                return Ok(new { usuario = new UsuarioDto(usuario) });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Elimina un usuario", Description = "Se elimina un usuario de la base de datos")]
        [SwaggerResponse(204, "Usuario eliminado")]
        [SwaggerResponse(404, "Usuario no encontrado")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var usuario = await Mdtecnologia.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                Mdtecnologia.Usuarios.Remove(usuario);
                await Mdtecnologia.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene todos los usuarios", Description = "Devuelve una lista de usuarios")]
        [SwaggerResponse(200, "Operación exitosa", typeof(List<UsuarioDto>))]
        public async Task<ActionResult<List<UsuarioDto>>> GetAll([FromQuery] int Limit = 25, [FromQuery] int Page = 0)
        {
            // TODO colocar filtrado
            if (Page < 0)
                return BadRequest(new { offset = "el número de página debe ser mayor o igual a 0" });
            if (Limit < 1)
                return BadRequest(new { limit = "la cantidad debe ser mayor a 0" });
            logger.Depuracion($"Pagination page: {Page} Limit {Limit}");
            
            var totalUsers = await Mdtecnologia.Usuarios.CountAsync();
            var hasNextPage = (Page + 1) * Limit < totalUsers;
            // Calcula la página anterior que contiene resultados
            int previousPage = Page - 1;
            while (previousPage > 0 && previousPage * Limit >= totalUsers)
                previousPage--;

            var nextUrl = hasNextPage
                ? Url.Action(
                    nameof(GetUsuario),
                    "Usuarios",
                    new
                    {
                        Limit,
                        Page = Page + 1
                    },
                    Request.Scheme)
                : null;

            var previousUrl = previousPage >= 0 && previousPage * Limit < totalUsers
                ? Url.Action(
                    nameof(GetUsuario),
                    "Usuarios",
                    new
                    {
                        Limit,
                        Page = previousPage
                    },
                    Request.Scheme
                ) : null;

            return Ok(new
            {
                count = totalUsers,
                next = nextUrl,
                previous = previousUrl,
                usuarios = await Mdtecnologia.Usuarios
                .OrderBy(u => u.Id)
                .Skip(Page * Limit)
                .Take(Limit)
                .Select(u => new UsuarioDto(u))
                .AsNoTracking()
                .ToListAsync()
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Obtiene un usuario por ID", Description = "Devuelve el detalle del usuario")]
        [SwaggerResponse(200, "Operación exitosa", typeof(UsuarioDto))]
        [SwaggerResponse(404, "Usuario no encontrado")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(Guid id)
        {
            var result = await Mdtecnologia.Usuarios.FindAsync(id);
            return result == null ? NotFound() : Ok(new { usuario = new UsuarioDto(result) });
        }
    }
}
