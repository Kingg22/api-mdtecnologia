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
        public required string Password { get; set; }

        public bool Disabled { get; set; } = false;

        public RolesEnum Rol { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}