﻿using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class UsuarioDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public string Username { get; set; } = null!;

        public bool Disabled { get; set; } = false;
        
        [StringLength(50)]
        public string Rol { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}