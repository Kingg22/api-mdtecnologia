using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class RestoreDto
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        [StringLength(70, MinimumLength = 8)]
        public required string Password { get; set; }
    }
}
