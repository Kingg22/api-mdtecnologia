using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace MD_Tech.Models;

[Table("trabajadores")]
public partial class Trabajador
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("apellido")]
    [StringLength(100)]
    public string Apellido { get; set; } = null!;

    [Column("correo")]
    [StringLength(255)]
    public string Correo { get; set; } = null!;

    [Column("telefono")]
    [StringLength(15)]
    public string? Telefono { get; set; }

    [Column("usuario")]
    public Guid? Usuario { get; set; }

    [Column("cargo")]
    [StringLength(100)]
    public string Cargo { get; set; } = null!;

    [Column("fecha_ingreso")]
    public LocalDate FechaIngreso { get; set; }

    [Column("estado")]
    public bool Estado { get; set; }

    [Column("salario")]
    [Precision(12, 2)]
    public decimal? Salario { get; set; }

    [Column("created_at")]
    public LocalDateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public LocalDateTime? UpdatedAt { get; set; }
}
