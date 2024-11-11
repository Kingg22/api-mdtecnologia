using NodaTime;

namespace MD_Tech.Models;

public partial class DireccionesClientes
{
    public Guid Cliente { get; set; }

    public Guid Direccion { get; set; }

    public LocalDateTime CreatedAt { get; set; }

    public virtual Clientes ClienteNavigation { get; set; } = null!;

    public virtual Direcciones DireccionNavigation { get; set; } = null!;
}
