namespace MD_Tech.Models;

public partial class DetallesVentas
{
    public Guid Id { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public Guid Producto { get; set; }

    public Guid Venta { get; set; }

    public virtual Productos ProductoNavigation { get; set; } = null!;

    public virtual Ventas VentaNavigation { get; set; } = null!;
}
