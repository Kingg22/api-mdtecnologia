using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ProductosDto
    {
        public Guid? Id { get; set; }

        [Required]
        public required string Nombre { get; set; }

        [Required]
        public required string Marca { get; set; }

        public string? Descripcion { get; set; }

        public Guid? Categoria { get; set; }

        // Representa una imagen guardada como archivo y con link interno de la api para obtenerla
        public string? Imagen1 { get; set; }

        // Representa la imagen de internet como link
        public string? Imagen2 { get; set; }

        public ICollection<ProductoProveedorDto>? Proveedores { get; set; } = [];
    }
}
