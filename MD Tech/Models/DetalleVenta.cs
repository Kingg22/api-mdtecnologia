using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MD_Tech.Models;

[Table("detalles_venta")]
public partial class DetalleVenta
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cantidad")]
    public int Cantidad { get; set; }

    [Column("precio_unitario")]
    [Precision(12, 2)]
    public decimal PrecioUnitario { get; set; }

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

    [Column("producto")]
    public Guid Producto { get; set; }

    [Column("venta")]
    public Guid Venta { get; set; }
}
