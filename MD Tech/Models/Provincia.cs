using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MD_Tech.Models;

[Table("provincias")]
public partial class Provincia
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("ProvinciaNavigation")]
    public virtual ICollection<Direccion> Direcciones { get; set; } = [];
}
