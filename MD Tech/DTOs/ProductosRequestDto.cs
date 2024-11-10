namespace MD_Tech.DTOs
{
    public class ProductosRequestDto
    {
        // Filtros de 1 solo valor, no se espera lista o exclusión de valores
        public string? Nombre { get; set; }

        public string? Marca { get; set; }
        
        public Guid? Categoria { get; set; }

        public int Limit { get; set; } = 25;

        public int Page { get; set; } = 0;
        
        // Formato esperado = precio-desc o precio-asc
        public string? OrderBy { get; set; }
    }
}
