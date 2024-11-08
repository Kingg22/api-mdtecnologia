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
    public class UsuariosController : ControllerBase
    {
        private readonly MdtecnologiaContext Mdtecnologia;
        private readonly RsaSecurityKey PrivateKey;
        private readonly LogsApi log;

        public UsuariosController(MdtecnologiaContext Mdtecnologia)
        {
            this.Mdtecnologia = Mdtecnologia;
            var rsa = RSA.Create();
            rsa.ImportFromPem(System.IO.File.ReadAllText("private.pem"));
            this.PrivateKey = new RsaSecurityKey(rsa);
            this.log = new LogsApi(typeof(UsuariosController));
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
                            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
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
                return BadRequest(new { password = "la contraseña no puede tener su usuario" });
            }
            var usuario = new Usuarios()
            {
                Id = Guid.NewGuid(),
                Username = register.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Rol = register.Rol.ToString(),
                Disabled = register.Disabled,
            };
            await Mdtecnologia.Usuarios.AddAsync(usuario);
            await Mdtecnologia.SaveChangesAsync();
            log.Informacion($"nuevo usuario creado ID: {usuario.Id}");
            return Created(Url.Action("GetUsuario", "Usuario", new { id = usuario.Id }, Request.Scheme), new
            {
                usuario =
                new UsuarioDto()
                {
                    Id = usuario.Id,
                    Username = usuario.Username,
                    Rol = usuario.Rol,
                    Disabled = usuario.Disabled,
                    CreatedAt = usuario.CreatedAt,
                    UpdatedAt = usuario.UpdatedAt,
                },
                warning = "debe relacionar el usuario"
            });
        }

        [HttpPatch("restore")]
        public async Task<ActionResult> ChangePassword([FromBody] RestoreDto restore)
        {
            var usuario = await Mdtecnologia.Usuarios.Where(u => u.Username.Equals(restore.Username)).FirstAsync();
            if (usuario == null)
            {
                return NotFound();
            }
            else
            {
                if (BCrypt.Net.BCrypt.Verify(restore.Password, usuario.Password))
                {
                    log.Informacion("cambio de contraseña rechazado por ser igual a la actual");
                    return BadRequest(new { password = "la nueva contraseña no puede ser igual a la actual" });
                }
                if (restore.Password.Contains(usuario.Username))
                {
                    log.Informacion("cambio de contraseña rechazado por contener su usuario");
                    return BadRequest(new { password = "la contraseña no puede tener su usuario" });
                }
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(restore.Password);
                await Mdtecnologia.SaveChangesAsync();
                log.Informacion("nueva contraseña creada");
                return Ok(new { message = "se ha cambiado su contraseña de forma exitosa" });
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult> UpdateProfile([FromBody] UsuarioUpdateDto usuarioDto)
        {
            var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUser == null || (idUser != null && idUser.Equals(usuarioDto.Id)))
            {
                return Unauthorized();
            }
            else
            {
                if (usuarioDto.Password.Contains(usuarioDto.Username))
                {
                    log.Informacion("cambio de contraseña rechazado por contener su usuario");
                    return BadRequest(new { password = "la contraseña no puede tener su usuario" });
                }
                var usuario = await Mdtecnologia.Usuarios.FindAsync(usuarioDto.Id);
                if (usuario == null)
                {
                    return NotFound();
                }
                else
                {
                    usuario.Username = usuarioDto.Username;
                    usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password);
                    usuario.Rol = usuarioDto.Rol.ToString();
                    await Mdtecnologia.SaveChangesAsync();
                    return Ok(new
                    {
                        usuario = new UsuarioDto()
                        {
                            Id = usuario.Id,
                            Username = usuario.Username,
                            Rol = usuario.Rol,
                            Disabled = usuario.Disabled,
                            CreatedAt = usuario.CreatedAt,
                            UpdatedAt = usuario.UpdatedAt,
                        }
                    });
                }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetAll([FromQuery] int limit = 25, [FromQuery] int page = 0)
        {
            // TODO colocar filtrado
            if (page < 0)
            {
                return BadRequest(new { offset = "el número de página debe ser mayor o igual a 0" });
            }
            if (limit < 1)
            {
                return BadRequest(new { limit = "la cantidad debe ser mayor a 0" });
            }
            log.Depuracion($"Pagination page: {page} Limit {limit}");
            var totalUsers = await Mdtecnologia.Usuarios.CountAsync();
            var hasNextPage = (page + 1) * limit < totalUsers;
            return Ok(new
            {
                count = totalUsers,
                next = hasNextPage ? Url.Action("GetAll", "Usuarios", new { limit, page = page + 1 }, Request.Scheme) : null,
                previous = page > 0 ? Url.Action("GetAll", "Usuarios", new { limit, page = page - 1 }, Request.Scheme) : null,
                usuarios = await Mdtecnologia.Usuarios
                .OrderBy(u => u.Id)
                .Skip(page * limit)
                .Take(limit)
                .Select(u => new UsuarioDto()
                {
                    Id = u.Id,
                    Username = u.Username,
                    Rol = u.Rol,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    Disabled = u.Disabled,
                }).ToListAsync()
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(Guid id)
        {
            var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUser == null || idUser != id.ToString())
            {
                return Unauthorized();
            }
            else
            {
                var result = await Mdtecnologia.Usuarios.FindAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(new
                    {
                        usuario = new UsuarioDto()
                        {
                            Id = result.Id,
                            Username = result.Username,
                            Disabled = result.Disabled,
                            Rol = result.Rol,
                            CreatedAt = result.CreatedAt,
                            UpdatedAt = result.UpdatedAt,
                        }
                    });
                }
            }
        }
    }
}
