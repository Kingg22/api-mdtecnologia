using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MD_Tech.Models;

[Table("categorias")]
public partial class Categoria
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("categoria_padre")]
    public Guid? CategoriaPadre { get; set; }

    [Column("imagen_url")]
    public string? ImagenUrl { get; set; }

    [ForeignKey("CategoriaPadre")]
    [InverseProperty("InverseCategoriaPadreNavigation")]
    public virtual Categoria? CategoriaPadreNavigation { get; set; }

    [InverseProperty("CategoriaPadreNavigation")]
    public virtual ICollection<Categoria> InverseCategoriaPadreNavigation { get; set; } = [];

    [InverseProperty("CategoriaNavigation")]
    public virtual ICollection<Producto> Productos { get; set; } = [];
}
