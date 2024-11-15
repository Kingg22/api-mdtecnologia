using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace MD_Tech.Models;

[Table("direccion_cliente")]
public partial class DireccionesCliente
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cliente")]
    public Guid Cliente { get; set; }

    [Column("direccion")]
    public Guid Direccion { get; set; }

    [Column("created_at")]
    public LocalDateTime CreatedAt { get; set; }

    [ForeignKey("Cliente")]
    [InverseProperty("DireccionClientes")]
    public virtual Cliente ClienteNavigation { get; set; } = null!;

    [ForeignKey("Direccion")]
    [InverseProperty("DireccionClientes")]
    public virtual Direccion DireccionNavigation { get; set; } = null!;
}
