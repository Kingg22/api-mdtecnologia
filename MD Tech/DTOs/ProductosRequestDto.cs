namespace MD_Tech.DTOs
{
    public class ProductosRequestDto : PaginationDto
    {
        /// <summary>
        /// Filtros de 1 solo valor, no se espera lista u operaciones
        /// </summary>
        public string? Nombre { get; set; }

        public string? Marca { get; set; }
        
        public Guid? Categoria { get; set; }
        
        /// <summary>
        /// Formato esperado = nombre-desc o nombre-asc o nombre
        /// </summary>
        public string? OrderBy { get; set; }
    }
}
