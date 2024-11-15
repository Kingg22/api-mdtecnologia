using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace MD_Tech.Models;

[Table("usuarios")]
[Index("Username", Name = "usuarios_username_key", IsUnique = true)]
public partial class Usuario
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(100)]
    public string? Password { get; set; }

    [Column("disabled")]
    public bool Disabled { get; set; }

    [Column("rol")]
    [StringLength(50)]
    public string Rol { get; set; } = null!;

    [Column("created_at")]
    public LocalDateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public LocalDateTime? UpdatedAt { get; set; }

    [InverseProperty("UsuarioNavigation")]
    public virtual Cliente? Cliente { get; set; }
}
