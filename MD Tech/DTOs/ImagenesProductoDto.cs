using System.ComponentModel.DataAnnotations;
using MD_Tech.Models;

namespace MD_Tech.DTOs
{
    public class ImagenesProductoDto
    {
        public Guid? Id { get; set; }

        public string? Descripcion { get; set; }

        [Required] 
        public string Url { get; set; } = null!;

        [Required]
        public Guid Producto { get; set; }

        public ImagenesProductoDto() { }

        public ImagenesProductoDto(ImagenesProducto img)
        {
            Id = img.Id;
            Url = img.Url;
            Descripcion = img.Descripcion;
            Producto = img.Producto;
        }
    }
}
