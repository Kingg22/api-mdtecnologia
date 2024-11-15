using MD_Tech.Models;
using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ProductosDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Marca { get; set; }

        public string? Descripcion { get; set; }

        public Guid? Categoria { get; set; }

        public ICollection<ProductoProveedorDto>? Proveedores { get; set; } = [];

        public ICollection<ImagenesProductoDto>? Imagenes { get; set; } = [];

        public ProductosDto() { Nombre = string.Empty; Marca = string.Empty; }

        public ProductosDto(Producto producto)
        {
            Id = producto.Id;
            Nombre = producto.Nombre;
            Marca = producto.Marca;
            Descripcion = producto.Descripcion;
            Categoria = producto.Categoria;

            Imagenes = producto.ImagenesProductos.Select(img => new ImagenesProductoDto()
            {
                Id = img.Id,
                Url = img.Url,
                Descripcion = img.Descripcion,
                Producto = img.Producto,
            }).ToList();

            Proveedores = producto.ProductosProveedores.Select(pp => new ProductoProveedorDto()
            {
                Producto = pp.Producto,
                Proveedor = pp.Proveedor,
                Precio = pp.Precio,
                Impuesto = pp.Impuesto,
                Total = pp.Total,
                FechaActualizado = pp.FechaActualizado,
                Stock = pp.Stock,
            }).ToList();
        }
    }
}
