using NodaTime;

namespace MD_Tech.Models;

public partial class HistorialProducto
{
    public Guid? Producto { get; set; }

    public decimal PrecioBaseAnterior { get; set; }

    public decimal PrecioTotalAnterior { get; set; }

    public LocalDateTime FechaCambio { get; set; }

    public virtual Productos? ProductoNavigation { get; set; }
}
