using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class DetalleVentaDto
    {
        public Guid? Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Range(0.00, double.MaxValue)]
        public decimal? PrecioUnitario { get; set; }

        [Range(0.00, double.MaxValue)]
        public decimal? Subtotal { get; set; }

        [Range(0.00, double.MaxValue)]
        public decimal? Descuento { get; set; }

        [Range(0.00, double.MaxValue)]
        public decimal? Impuesto { get; set; }

        [Range(0.00, double.MaxValue)]
        public decimal? Total { get; set; }

        [Required]
        public Guid Producto { get; set; }

        public Guid? Venta { get; set; }
    }
}
