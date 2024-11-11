using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ProveedoresDto
    {
        public Guid? Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Nombre { get; set; }

        [StringLength(255, MinimumLength = 1)]
        public string? Correo { get; set; }

        [StringLength (15, MinimumLength = 1)]
        public string? Telefono { get; set; }

        public DireccionDto? Direccion { get; set; }
    }
}
