using MD_Tech.Models;
using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ClienteDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Apellido { get; set; } = null!;

        [StringLength(255, MinimumLength = 1)]
        public string Correo { get; set; } = null!;

        public string? Telefono { get; set; }

        [Required]
        public Guid? IdUsuario { get; set; }

        public ICollection<DireccionDto> Direcciones { get; set; } = [];

        public ClienteDto() { }

        public ClienteDto(Cliente cliente)
        {
            Id = cliente.Id;
            Nombre = cliente.Nombre;
            Apellido = cliente.Apellido;
            Correo = cliente.Correo;
            Telefono = cliente.Telefono;
            IdUsuario = cliente.Usuario;
            Direcciones = cliente.Direcciones.Select(d => new DireccionDto()
            {
                Id = d.Id,
                Descripcion = d.Descripcion,
                CreatedAt = d.CreatedAt,
                Provincia = d.Provincia,
            }).ToList();
        }
    }
}
