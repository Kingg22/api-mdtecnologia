namespace MD_Tech.Models;

public partial class DireccionesClientes
{
    public Guid Cliente { get; set; }

    public Guid Direccion { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Clientes ClienteNavigation { get; set; } = null!;

    public virtual Direcciones DireccionNavigation { get; set; } = null!;
}
