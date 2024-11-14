using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace MD_Tech.Models;

[Table("historial_productos")]
public partial class HistorialProducto
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("producto")]
    public Guid? Producto { get; set; }

    [Column("precio_base_anterior")]
    [Precision(12, 2)]
    public decimal PrecioBaseAnterior { get; set; }

    [Column("precio_total_anterior")]
    [Precision(12, 2)]
    public decimal PrecioTotalAnterior { get; set; }

    [Column("fecha_cambio")]
    public LocalDateTime FechaCambio { get; set; }

    [ForeignKey("Producto")]
    [InverseProperty("HistorialProductos")]
    public virtual Producto? ProductoNavigation { get; set; }
}
