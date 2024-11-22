using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{ 
    public class VentaDto {

        public Guid? Id { get; set; }

        public LocalDateTime? Fecha { get; set; }

        public EstadoVentaEnum Estado { get; set; } = EstadoVentaEnum.PROCESANDO;

        [Range(0, int.MaxValue)]
        public int? CantidadTotalProductos { get; set; }

        [Range(0.00, double.MaxValue)]
        public decimal? Subtotal { get; set; }

        [Range(0.00, double.MaxValue)]
        public decimal? Descuento { get; set; }

        [Range(0.00, double.MaxValue)]
        public decimal? Impuesto { get; set; }

        [Range(0.00, double.MaxValue)]
        public decimal? Total { get; set; }

        [Required]
        public Guid Cliente { get; set; }

        [Required]
        public Guid DireccionEntrega { get; set; }

        public ICollection<DetalleVentaDto> Detalles { get; set; } = [];

    }

}
