using NodaTime;

namespace MD_Tech.Models;

public partial class Ventas
{
    public Guid Id { get; set; }

    public LocalDateTime Fecha { get; set; }

    public string Estado { get; set; } = null!;

    public int CantidadTotalProductos { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public Guid DireccionEntrega { get; set; }

    public Guid Cliente { get; set; }

    public virtual Clientes ClienteNavigation { get; set; } = null!;

    public virtual ICollection<DetallesVentas> DetallesVenta { get; set; } = [];

    public virtual Direcciones DireccionEntregaNavigation { get; set; } = null!;
}
