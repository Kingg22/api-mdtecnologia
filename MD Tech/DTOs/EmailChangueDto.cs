using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{ 
    public class EmailChangueDto 
    {
        [Required]
        public required Guid Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public required string Correo { get; set; }

    }

}
