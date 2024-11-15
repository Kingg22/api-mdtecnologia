using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace MD_Tech.Models;

[Table("productos_proveedores")]
public partial class ProductosProveedor
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("producto")]
    public Guid Producto { get; set; }

    [Column("proveedor")]
    public Guid Proveedor { get; set; }

    [Column("precio")]
    [Precision(12, 2)]
    public decimal Precio { get; set; }

    [Column("impuesto")]
    [Precision(12, 2)]
    public decimal Impuesto { get; set; }

    [Column("total")]
    [Precision(12, 2)]
    public decimal Total { get; set; }

    [Column("stock")]
    public int? Stock { get; set; }

    [Column("fecha_actualizado")]
    public LocalDate FechaActualizado { get; set; }

    [ForeignKey("Producto")]
    [InverseProperty("ProductosProveedores")]
    public virtual Producto ProductoNavigation { get; set; } = null!;

    [ForeignKey("Proveedor")]
    [InverseProperty("ProductosProveedores")]
    public virtual Proveedor ProveedorNavigation { get; set; } = null!;
}
