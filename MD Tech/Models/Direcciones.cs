using NodaTime;

namespace MD_Tech.Models;

public partial class Direcciones
{
    public Guid Id { get; set; }

    public string? Descripcion { get; set; }

    public int Provincia { get; set; }

    public LocalDateTime CreatedAt { get; set; }

    public virtual ICollection<Proveedores> Proveedores { get; set; } = [];

    public virtual Provincia ProvinciaNavigation { get; set; } = null!;

    public virtual ICollection<Ventas> Ventas { get; set; } = [];
}
