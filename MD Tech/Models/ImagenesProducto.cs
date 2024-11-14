using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MD_Tech.Models;

[Table("imagenes_productos")]
public partial class ImagenesProducto
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("url")]
    public string Url { get; set; } = null!;

    [Column("producto")]
    public Guid Producto { get; set; }

    [ForeignKey("Producto")]
    [InverseProperty("ImagenesProductos")]
    public virtual Producto ProductoNavigation { get; set; } = null!;
}
