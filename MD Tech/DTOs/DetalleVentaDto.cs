using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class DetalleVentaDto
    {
        public Guid? Id { get; set; }

        [Required]
        public required int Cantidad { get; set; }

        public decimal? PrecioUnitario { get; set; }

        public decimal? Subtotal { get; set; }

        public decimal? Descuento { get; set; }

        public decimal? Impuesto { get; set; }

        public decimal? Total { get; set; }

        [Required]
        public Guid Producto { get; set; }

        public Guid? Venta { get; set; }
    }
}
