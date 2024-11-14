using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace MD_Tech.Models;

[Table("direcciones")]
public partial class Direccion
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("provincia")]
    public int Provincia { get; set; }

    [Column("created_at")]
    public LocalDateTime CreatedAt { get; set; }

    [InverseProperty("DireccionNavigation")]
    public virtual ICollection<DireccionesCliente> DireccionClientes { get; set; } = [];

    public virtual ICollection<Cliente> Clientes { get; set; } = [];

    [InverseProperty("DireccionNavigation")]
    public virtual ICollection<Proveedor> Proveedores { get; set; } = [];

    [ForeignKey("Provincia")]
    [InverseProperty("Direcciones")]
    public virtual Provincia ProvinciaNavigation { get; set; } = null!;
}
