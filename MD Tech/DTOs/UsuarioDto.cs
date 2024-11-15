using MD_Tech.Models;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class UsuarioDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public string Username { get; set; } = null!;

        public bool Disabled { get; set; } = false;
        
        [StringLength(50)]
        public string Rol { get; set; } = null!;

        public LocalDateTime? CreatedAt { get; set; }

        public LocalDateTime? UpdatedAt { get; set; }
    
        public UsuarioDto() { }

        public UsuarioDto(Usuario usuario)
        {
            Id = usuario.Id;
            Username = usuario.Username;
            Disabled = usuario.Disabled;
            Rol = usuario.Rol;
            CreatedAt = usuario.CreatedAt;
            UpdatedAt = usuario.UpdatedAt;
        }
    }
}
