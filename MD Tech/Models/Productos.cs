namespace MD_Tech.Models;

public partial class Productos
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string? Descripcion { get; set; }

    public Guid? Categoria { get; set; }

    public byte[]? ImagenFile { get; set; }

    public string? ImagenUrl { get; set; }

    public virtual Categorias? CategoriaNavigation { get; set; }

    public virtual ICollection<DetallesVentas> DetallesVenta { get; set; } = [];

    public virtual ICollection<ProductosProveedor> ProductosProveedores { get; set; } = [];
}
