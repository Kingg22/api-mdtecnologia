using MD_Tech.Models;
using NodaTime;

namespace MD_Tech.DTOs
{
    public class DireccionDto
    {
        public Guid? Id { get; set; }

        public string? Descripcion { get; set; }

        public int? Provincia { get; set; }

        public LocalDateTime? CreatedAt { get; set; }

        public DireccionDto() { }

        public DireccionDto(Direccion direccion)
        {
            Id = direccion.Id;
            Descripcion = direccion.Descripcion;
            Provincia = direccion.Provincia;
            CreatedAt = direccion.CreatedAt;
        }

        public DireccionDto(Guid id, Direccion? direccion)
        {
            Id = direccion?.Id == null ? id : direccion.Id;
            Descripcion = direccion?.Descripcion;
            Provincia= direccion?.Provincia;
            CreatedAt= direccion?.CreatedAt;
        }
    }
}
