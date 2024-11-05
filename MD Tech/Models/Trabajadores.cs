namespace MD_Tech.Models;

public partial class Trabajadores
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public Guid? Usuario { get; set; }

    public string Cargo { get; set; } = null!;

    public DateOnly FechaIngreso { get; set; }

    public bool Estado { get; set; }

    public decimal? Salario { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Usuarios? UsuarioNavigation { get; set; }
}
