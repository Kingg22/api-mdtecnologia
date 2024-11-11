using NodaTime;

namespace MD_Tech.Models;

public partial class ProductosProveedor
{
    public Guid Producto { get; set; }

    public Guid Proveedor { get; set; }

    public decimal Precio { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public int? Stock { get; set; }

    public LocalDate FechaActualizado { get; set; }

    public virtual Productos ProductoNavigation { get; set; } = null!;

    public virtual Proveedores ProveedorNavigation { get; set; } = null!;
}
