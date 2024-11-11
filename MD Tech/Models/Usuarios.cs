﻿using NodaTime;

namespace MD_Tech.Models;

public partial class Usuarios
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public bool Disabled { get; set; }

    public string Rol { get; set; } = null!;

    public LocalDateTime CreatedAt { get; set; }

    public LocalDateTime? UpdatedAt { get; set; }

    public virtual Clientes? Cliente { get; set; }

    public virtual Trabajadores? Trabajador { get; set; }
}
