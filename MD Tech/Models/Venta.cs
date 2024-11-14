using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace MD_Tech.Models;

[Table("ventas")]
public partial class Venta
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("fecha")]
    public LocalDateTime Fecha { get; set; }

    [Column("estado")]
    [StringLength(50)]
    public string Estado { get; set; } = null!;

    [Column("cantidad_total_productos")]
    public int CantidadTotalProductos { get; set; }

    [Column("subtotal")]
    [Precision(12, 2)]
    public decimal Subtotal { get; set; }

    [Column("descuento")]
    [Precision(12, 2)]
    public decimal Descuento { get; set; }

    [Column("impuesto")]
    [Precision(12, 2)]
    public decimal Impuesto { get; set; }

    [Column("total")]
    [Precision(12, 2)]
    public decimal Total { get; set; }

    [Column("direccion_entrega")]
    public Guid DireccionEntrega { get; set; }

    [Column("cliente")]
    public Guid Cliente { get; set; }
}
