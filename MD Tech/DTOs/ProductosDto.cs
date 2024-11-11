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

        public ICollection<ProductoProveedorDto>? Proveedores { get; set; }

        public ICollection<ImagenesProductoDto>? Imagenes { get; set; }
    }
}
