using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(70, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        public string Rol { get; set; } = null!;

        public bool Disabled { get; set; } = false;

    }
}
