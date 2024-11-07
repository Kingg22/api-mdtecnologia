using BCrypt.Net;
using MD_Tech.Contexts;
using MD_Tech.DTOs;
using MD_Tech.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MD_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly MdtecnologiaContext Mdtecnologia;
        private readonly RsaSecurityKey PrivateKey;
        private readonly LogsApi log;

        public UsuarioController(MdtecnologiaContext Mdtecnologia)
        {
            this.Mdtecnologia = Mdtecnologia;
            var rsa = RSA.Create();
            rsa.ImportFromPem(System.IO.File.ReadAllText("private.pem"));
            this.PrivateKey = new RsaSecurityKey(rsa);
            this.log = new LogsApi(typeof(UsuarioController));
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var query = Mdtecnologia.Usuarios.Where(u => u.Username.Equals(loginRequest.Email));
            if (await query.AnyAsync())
            {
                var usuario = await query.FirstAsync();
                if (usuario?.Password != null)
                {
                    log.Depuracion($"Usuario ID: {usuario.Id}");
                    var passMatch = BCrypt.Net.BCrypt.Verify(loginRequest.Password, usuario.Password);
                    log.Depuracion($"Bcrypt verify: {passMatch}");
                    if (passMatch && !usuario.Disabled)
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, usuario.Id.ToString()),
                            new Claim(ClaimTypes.Role, usuario.Rol)
                        };
                        var tokenDes = new SecurityTokenDescriptor()
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.UtcNow.AddHours(1),
                            SigningCredentials = new SigningCredentials(PrivateKey, SecurityAlgorithms.RsaSha256),
                            IssuedAt = DateTime.UtcNow,
                            Issuer = "MDTech",
                            NotBefore = DateTime.UtcNow
                        };
                        var jwtToken = tokenHandler.CreateToken(tokenDes);
                        log.Informacion($"Se ha generado un nuevo token al usuario: {usuario.Id}");
                        return Ok(new { token = tokenHandler.WriteToken(jwtToken) });
                    }
                    else
                    {
                        log.Informacion("login fallido: la contraseña no coincide y/o usuario deshabilitado");
                    }
                }
                else
                {
                    log.Informacion("login fallido: el usuario no tiene contraseña");
                }
            }
            else
            {
                log.Informacion("login fallido: no se encontró usuario en la BD");
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<ActionResult<UsuarioDto>> CreateUser([FromBody] RegisterUserDto register)
        {
            if (await Mdtecnologia.Usuarios.Where(u => u.Username.Equals(register.Username)).AnyAsync())
            {
                log.Informacion("registro rechazado ya existe ese username");
                return BadRequest(new { username = "nombre de usuario en uso, intente nuevamente" });
            }
            if (register.Password.Contains(register.Username))
            {
                log.Informacion("registro rechazado por contraseña contiene usuario");
                return BadRequest(new {password = "la contraseña no puede tener su usuario"});
            }
            var usuario = new Usuarios()
            {
                Id = Guid.NewGuid(),
                Username = register.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Rol = register.Rol,
                Disabled = register.Disabled,
            };
            await Mdtecnologia.Usuarios.AddAsync(usuario);
            await Mdtecnologia.SaveChangesAsync();
            var result = await Mdtecnologia.Usuarios.FindAsync(usuario.Id);
            if (result == null)
            {
                log.Errores($"No se encontró la entidad {usuario.Id} después de insertarla en la BD");
                return StatusCode(500, "Hubo un error al recuperar la entidad después de insertarse, verificar antes de intentar");
            }
            var output = new UsuarioDto()
            {
                Id = result.Id,
                Username = result.Username,
                Rol = result.Rol,
                Disabled = result.Disabled,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt,
            };
            log.Informacion($"nuevo usuario creado ID: {result.Id}");
            return Created(Url.Action("GetUsuario", "Usuario", new { id = result.Id }, Request.Scheme), output);
        }

        [HttpPatch("restore")]
        public async Task<ActionResult> ChangePassword([FromBody] RestoreDto restore)
        {
            var usuario = await Mdtecnologia.Usuarios.FindAsync(restore.Id);
            if (usuario == null)
            {
                return NotFound();
            } 
            else
            {
                if (restore.Username.Equals(usuario.Username))
                {
                    if (BCrypt.Net.BCrypt.Verify(restore.Password, usuario.Password))
                    {
                        log.Informacion("cambio de contraseña rechazado por ser igual a la actual");
                        return BadRequest(new {password = "la nueva contraseña no puede ser igual a la actual"});
                    }
                    if (restore.Password.Contains(usuario.Username))
                    {
                        log.Informacion("cambio de contraseña rechazado por contener su usuario");
                        return BadRequest(new {password = "la contraseña no puede tener su usuario"});
                    } 
                    usuario.Password = BCrypt.Net.BCrypt.HashPassword(restore.Password);
                    await Mdtecnologia.SaveChangesAsync();
                    log.Informacion("nueva contraseña creada");
                    return Ok();
                } else
                {
                    log.Informacion($"cambio de contraseña rechazado el usuario no coincide con el ID {usuario.Id}");
                    return BadRequest(new {username = "los datos no coinciden"});
                }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Usuarios>>> GetAll([FromQuery] int limit = 25, [FromQuery] int offset = 0)
        {
            if (offset < 0)
            {
                return BadRequest(new { offset = "el número de página debe ser mayor o igual a 0" });
            }
            if (limit < 1)
            {
                return BadRequest(new { limit = "la cantidad debe ser mayor a 0"});
            }
            log.Depuracion($"Pagination: {offset} Limit {limit}");
            return await Mdtecnologia.Usuarios.OrderBy(u => u.Id).Skip(offset * limit).Take(limit).ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(Guid id)
        {
            var result = await Mdtecnologia.Usuarios.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(new UsuarioDto()
                {
                    Id = result.Id,
                    Username = result.Username,
                    Disabled = result.Disabled,
                    Rol = result.Rol,
                    CreatedAt = result.CreatedAt,
                    UpdatedAt = result.UpdatedAt,
                });
            }
        }
    }
}
