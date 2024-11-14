using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MD_Tech.Models;

[Table("contacto_proveedor")]
public partial class ContactoProveedor
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    public string? Nombre { get; set; }

    [Column("correo")]
    [StringLength(255)]
    public string? Correo { get; set; }

    [Column("telefono")]
    [StringLength(15)]
    public string? Telefono { get; set; }

    [Column("proveedor")]
    public Guid Proveedor { get; set; }

    [ForeignKey("Proveedor")]
    [InverseProperty("ContactoProveedors")]
    public virtual Proveedor ProveedorNavigation { get; set; } = null!;
}
