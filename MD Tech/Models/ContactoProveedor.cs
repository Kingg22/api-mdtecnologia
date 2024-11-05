namespace MD_Tech.Models;

public partial class ContactoProveedor
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public Guid Proveedor { get; set; }

    public virtual Proveedores ProveedorNavigation { get; set; } = null!;
}
