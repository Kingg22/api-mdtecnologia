using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ClienteUsuarioDto
    {
        public Guid? UsuarioID { get; set; }

        public Guid? ClienteID { get; set; }
        
        [Required]
        [StringLength(70, MinimumLength = 8)]
        public required string Password { get; set; }

        public bool Disabled { get; set; } = false;

        [Required]
        public required string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Nombre { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Apellido { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public required string Correo { get; set; }

        public string? Telefono { get; set; }

    }
}
