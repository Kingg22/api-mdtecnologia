using MD_Tech.Models;
using System.ComponentModel.DataAnnotations;

namespace MD_Tech.DTOs
{
    public class CategoriaDto
    {
        public Guid? Id { get; set; }

        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public Guid? CategoriaPadre { get; set; }

        public string? ImagenUrl { get; set; }

        public CategoriaDto() { }

        public CategoriaDto(Categoria categoria)
        {
            Id = categoria.Id;
            Nombre = categoria.Nombre;
            Descripcion = categoria.Descripcion;
            CategoriaPadre = categoria.CategoriaPadre;
            ImagenUrl = categoria.ImagenUrl;
        }
    }
}
