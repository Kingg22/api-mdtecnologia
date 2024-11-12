﻿using NodaTime;

namespace MD_Tech.Models;

public partial class Clientes
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public Guid? Usuario { get; set; }

    public LocalDateTime CreatedAt { get; set; }

    public LocalDateTime? UpdatedAt { get; set; }

    public virtual Usuarios? UsuarioNavigation { get; set; }

    public virtual ICollection<DireccionesClientes> DireccionesClientes { get; set; } = [];

    public virtual ICollection<Direcciones> Direcciones { get; set; } = [];

    public virtual ICollection<Ventas> Ventas { get; set; } = [];
}
