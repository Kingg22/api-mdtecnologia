using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class UsuarioUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(70, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        public bool Disabled { get; set; } = false;

        public RolesEnum Rol { get; set; }

        public LocalDateTime? CreatedAt { get; set; }

        public LocalDateTime? UpdatedAt { get; set; }
    }
}