using MD_Tech.Models;
using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class ProveedoresDto
    {
        public Guid? Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Nombre { get; set; } = null!;

        [StringLength(255, MinimumLength = 1)]
        public string? Correo { get; set; }

        [StringLength (15, MinimumLength = 1)]
        public string? Telefono { get; set; }

        public DireccionDto? Direccion { get; set; }

        public ProveedoresDto() { }

        public ProveedoresDto(Proveedor proveedor)
        {
            Id = proveedor.Id;
            Nombre = proveedor.Nombre;
            Correo = proveedor.Correo;
            Telefono = proveedor.Telefono;
            Direccion = proveedor.Direccion == null ? null : new DireccionDto(proveedor.Id, proveedor.DireccionNavigation);
        }
    }
}
