using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace MD_Tech.Models;

[Table("ordenes_compra_proveedor")]
public partial class OrdenesCompraProveedor
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("proveedor")]
    public Guid Proveedor { get; set; }

    [Column("id_orden")]
    public string IdOrden { get; set; } = null!;

    [Column("fecha_estimada_entrega")]
    public LocalDate? FechaEstimadaEntrega { get; set; }

    [Column("estado")]
    [StringLength(50)]
    public string Estado { get; set; } = null!;

    [ForeignKey("Proveedor")]
    [InverseProperty("OrdenesCompraProveedors")]
    public virtual Proveedor ProveedorNavigation { get; set; } = null!;
}
