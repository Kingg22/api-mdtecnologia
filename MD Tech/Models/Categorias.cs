namespace MD_Tech.Models;

public partial class Categorias
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Productos> Productos { get; set; } = [];
}
