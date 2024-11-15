using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MD_Tech.Models;

[Table("proveedores")]
[Index("Correo", Name = "proveedores_correo_key", IsUnique = true)]
[Index("Telefono", Name = "proveedores_telefono_key", IsUnique = true)]
public partial class Proveedor
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("direccion")]
    public Guid? Direccion { get; set; }

    [Column("correo")]
    [StringLength(255)]
    public string? Correo { get; set; }

    [Column("telefono")]
    [StringLength(15)]
    public string? Telefono { get; set; }

    [InverseProperty("ProveedorNavigation")]
    public virtual ICollection<ContactoProveedor> ContactoProveedors { get; set; } = [];

    [ForeignKey("Direccion")]
    [InverseProperty("Proveedores")]
    public virtual Direccion? DireccionNavigation { get; set; }

    [InverseProperty("ProveedorNavigation")]
    public virtual ICollection<OrdenesCompraProveedor> OrdenesCompraProveedors { get; set; } = [];

    [InverseProperty("ProveedorNavigation")]
    public virtual ICollection<ProductosProveedor> ProductosProveedores { get; set; } = [];
}
