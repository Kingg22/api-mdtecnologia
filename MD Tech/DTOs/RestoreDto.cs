using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class RestoreDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(70, MinimumLength = 8)]
        public string Password { get; set; } = null!;
    }
}
