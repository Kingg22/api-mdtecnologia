using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class RegisterUserDto
    {
        public Guid? Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(70, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        public RolesEnum Rol { get; set; }

        public bool Disabled { get; set; } = false;

    }
}
