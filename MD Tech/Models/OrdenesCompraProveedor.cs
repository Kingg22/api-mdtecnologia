namespace MD_Tech.Models;

public partial class OrdenesCompraProveedor
{
    public Guid Id { get; set; }

    public Guid Proveedor { get; set; }

    public string IdOrden { get; set; } = null!;

    public DateOnly? FechaEstimadaEntrega { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Proveedores ProveedorNavigation { get; set; } = null!;
}
