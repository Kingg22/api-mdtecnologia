using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ImagenesProductoDto
    {
        public Guid? Id { get; set; }

        public string? Descripcion { get; set; }

        [Required]
        public required string Url { get; set; }

        [Required]
        public Guid Producto { get; set; }
    }
}
