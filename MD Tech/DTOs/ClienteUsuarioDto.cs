using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ClienteUsuarioDto
    {
        public Guid? UsuarioId { get; set; }

        public Guid? ClienteId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Apellido { get; set; } = null!;

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Correo { get; set; } = null!;

        public string? Telefono { get; set; }

        public ICollection<DireccionDto> Direcciones { get; set; } = [];

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(70, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        public bool Disabled { get; set; } = false;
    }
}
