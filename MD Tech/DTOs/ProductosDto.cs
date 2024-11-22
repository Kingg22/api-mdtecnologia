using MD_Tech.Models;
using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ProductosDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Marca { get; set; } = null!;

        public string? Descripcion { get; set; }

        public Guid? Categoria { get; set; } 

        public ICollection<ProductoProveedorDto>? Proveedores { get; set; } = [];

        public ICollection<ImagenesProductoDto>? Imagenes { get; set; } = [];

        public ProductosDto() { }

        public ProductosDto(Producto producto)
        {
            Id = producto.Id;
            Nombre = producto.Nombre;
            Marca = producto.Marca;
            Descripcion = producto.Descripcion;
            Categoria = producto.Categoria;

            Imagenes = producto.ImagenesProductos.Select(img => new ImagenesProductoDto(img)).ToList();

            Proveedores = producto.ProductosProveedores
                .OrderBy(pp => pp.Total)
                .Select(pp => new ProductoProveedorDto(pp)).ToList();
        }
    }
}
