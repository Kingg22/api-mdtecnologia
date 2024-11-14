using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MD_Tech.Models;

[Table("productos")]
public partial class Producto
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = null!;

    [Column("marca")]
    public string Marca { get; set; } = null!;

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("categoria")]
    public Guid? Categoria { get; set; }

    [ForeignKey("Categoria")]
    [InverseProperty("Productos")]
    public virtual Categoria? CategoriaNavigation { get; set; }

    [InverseProperty("ProductoNavigation")]
    public virtual ICollection<HistorialProducto> HistorialProductos { get; set; } = [];

    [InverseProperty("ProductoNavigation")]
    public virtual ICollection<ImagenesProducto> ImagenesProductos { get; set; } = [];

    [InverseProperty("ProductoNavigation")]
    public virtual ICollection<ProductosProveedor> ProductosProveedores { get; set; } = [];
}
