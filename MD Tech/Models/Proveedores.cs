namespace MD_Tech.Models;

public partial class Proveedores
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public Guid? Direccion { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<ContactoProveedor> ContactoProveedors { get; set; } = [];

    public virtual Direcciones? DireccionNavigation { get; set; }

    public virtual ICollection<OrdenesCompraProveedor> OrdenesCompraProveedores { get; set; } = [];

    public virtual ICollection<ProductosProveedor> ProductosProveedores { get; set; } = [];
}
