using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{ 
    public class VentaDto {

        public Guid? Id { get; set; }

        public LocalDateTime? Fecha { get; set; }

        public EstadoVentaEnum Estado { get; set; } = EstadoVentaEnum.PROCESANDO;

        public int? CantidadTotalProductos { get; set; }

        public decimal? Subtotal { get; set; }

        public decimal? Descuento { get; set; }

        public decimal? Impuesto { get; set; }

        public decimal? Total { get; set; }

        public Guid? DireccionEntrega { get; set; }

        [Required]
        public Guid Cliente { get; set; }

        public ICollection<DetalleVentaDto> Detalles { get; set; } = [];

    }

}
