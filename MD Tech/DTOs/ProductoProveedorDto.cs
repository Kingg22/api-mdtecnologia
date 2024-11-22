using NodaTime;
using System.ComponentModel.DataAnnotations;
using MD_Tech.Models;

namespace MD_Tech.DTOs
{
    public class ProductoProveedorDto
    {
        public Guid? Producto { get; set; }
        
        [Required]
        public Guid Proveedor { get; set; }

        [Required]
        [Range(0.00, double.MaxValue)]
        public decimal Precio { get; set; }

        [Required]
        [Range(0.00, double.MaxValue)]
        public decimal Impuesto { get; set; } = decimal.Zero;

        [Range(0.00, double.MaxValue)]
        public decimal? Total { get; set; }

        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }

        public LocalDate? FechaActualizado { get; set; }

        public ProductoProveedorDto() { }

        public ProductoProveedorDto(ProductosProveedor pp)
        {
            Producto = pp.Producto;
            Proveedor = pp.Proveedor;
            Precio = pp.Precio;
            Impuesto = pp.Impuesto;
            Total = pp.Total;
            FechaActualizado = pp.FechaActualizado;
            Stock = pp.Stock;
        }
    }
}
