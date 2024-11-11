using NodaTime;

namespace MD_Tech.DTOs
{
    public class DireccionDto
    {
        public Guid? Id { get; set; }

        public string? Descripcion { get; set; }

        public int? Provincia { get; set; }

        public LocalDateTime? CreatedAt { get; set; }
    }
}
