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
            PrivateKey = new RsaSecurityKey(rsa);
            log = new LogsApi(typeof(UsuariosController));
        }

        [HttpPost("login")]
        public async Task<ActionResult<Dictionary<string, string>>> Login([FromBody] LoginRequest loginRequest)
        {
            var usuario = await Mdtecnologia.Usuarios.FirstOrDefaultAsync(u => u.Username.Equals(loginRequest.Email));
            if (usuario != null)
            {
                if (usuario.Password != null)
                {
                    log.Depuracion($"Usuario ID: {usuario.Id}");
                    var passMatch = BCrypt.Net.BCrypt.Verify(loginRequest.Password, usuario.Password);
                    log.Depuracion($"Bcrypt verify: {passMatch}");
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
            if (await Mdtecnologia.Usuarios.AnyAsync(u => u.Username.Equals(register.Username)))
            {
                log.Informacion("registro rechazado ya existe ese username");
                return BadRequest(new { username = "nombre de usuario en uso, intente nuevamente" });
            }
            if (string.IsNullOrWhiteSpace(register.Password) || register.Password.Contains(register.Username))
            {
                log.Informacion("registro rechazado por contraseña contiene usuario");
                return BadRequest(new { password = "ingrese una contraseña válida" });
            }
            if (string.IsNullOrWhiteSpace(register.Username))
            {
                log.Informacion("No proporcionó un username");
                return BadRequest(new { username = "el username es requerido" });
            }
            if (register.Id != null && await Mdtecnologia.Usuarios.FindAsync(register.Id) != null)
            {
                return BadRequest(new { Id = "id proporcionado en uso" });
            }
            var usuario = new Usuarios()
            {
                Id = register.Id ?? Guid.NewGuid(),
                Username = register.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Rol = register.Rol.ToString(),
                Disabled = register.Disabled,
            };
            await Mdtecnologia.Usuarios.AddAsync(usuario);
            await Mdtecnologia.SaveChangesAsync();
            log.Informacion($"nuevo usuario creado ID: {usuario.Id}");
            return Created(Url.Action(nameof(GetUsuario), "Usuarios", new { id = usuario.Id }, Request.Scheme), new
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

        [HttpPatch("restore-password")]
        public async Task<ActionResult<Dictionary<string, string>>> ChangePassword([FromBody] RestoreDto restore)
        {
            var usuario = await Mdtecnologia.Usuarios.FirstOrDefaultAsync(u => u.Username.Equals(restore.Username));
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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> UpdateProfile(Guid id, [FromBody] UsuarioUpdateDto usuarioDto)
        {
            if (id != usuarioDto.Id)
            {
                log.Informacion($"usuario de id {id} no coincide con el body id {usuarioDto.Id}");
                return BadRequest();
            }
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

        [HttpDelete("{id}")]
        [Authorize]
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
        public async Task<ActionResult<List<UsuarioDto>>> GetAll([FromQuery] int limit = 25, [FromQuery] int page = 0)
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
                next = hasNextPage ? Url.Action(nameof(GetAll), "Usuarios", new { limit, page = page + 1 }, Request.Scheme) : null,
                previous = page > 0 ? Url.Action(nameof(GetAll), "Usuarios", new { limit, page = page - 1 }, Request.Scheme) : null,
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
                })
                .AsNoTracking()
                .ToListAsync()
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(Guid id)
        {
            var result = await Mdtecnologia.Usuarios.FindAsync(id);
            return result == null ? NotFound() : Ok(new
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
