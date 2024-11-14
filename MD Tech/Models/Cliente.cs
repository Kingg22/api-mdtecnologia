using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace MD_Tech.Models;

[Table("clientes")]
[Index("Correo", Name = "clientes_correo_key", IsUnique = true)]
[Index("Telefono", Name = "clientes_telefono_key", IsUnique = true)]
[Index("Usuario", Name = "clientes_usuario_key", IsUnique = true)]
public partial class Cliente
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("apellido")]
    [StringLength(100)]
    public string Apellido { get; set; } = null!;

    [Column("correo")]
    [StringLength(255)]
    public string Correo { get; set; } = null!;

    [Column("telefono")]
    [StringLength(15)]
    public string? Telefono { get; set; }

    [Column("usuario")]
    public Guid? Usuario { get; set; }

    [Column("created_at")]
    public LocalDateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public LocalDateTime? UpdatedAt { get; set; }

    [InverseProperty("ClienteNavigation")]
    public virtual ICollection<DireccionesCliente> DireccionClientes { get; set; } = [];

    public virtual ICollection<Direccion> Direcciones { get; set; } = [];

    [ForeignKey("Usuario")]
    [InverseProperty("Cliente")]
    public virtual Usuario? UsuarioNavigation { get; set; }
}
