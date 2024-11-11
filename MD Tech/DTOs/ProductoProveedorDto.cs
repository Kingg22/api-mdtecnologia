using NodaTime;
using System.ComponentModel.DataAnnotations;

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

        public decimal? Total { get; set; }

        public int? Stock { get; set; }

        public LocalDate? FechaActualizado { get; set; }
    }
}
