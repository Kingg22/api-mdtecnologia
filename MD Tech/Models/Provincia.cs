namespace MD_Tech.Models;

public partial class Provincia
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Direcciones> Direcciones { get; set; } = [];
}
