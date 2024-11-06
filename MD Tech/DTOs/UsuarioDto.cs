namespace MD_Tech.DTOs
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public bool Disabled { get; set; }

        public string Rol { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
