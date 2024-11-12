using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ClienteDto
    {
        public Guid? Id { get; set; }
        
        [Required]
        public required string Nombre { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Apellido { get; set; }

        [StringLength(255,MinimumLength =1)]        
        public required string Correo { get; set; }

        public string? Telefono { get; set; }

        [Required]
        public Guid? IdUsuario { get; set; }

        public ICollection<DireccionDto> Direcciones { get; set; } = [];
    }
}
