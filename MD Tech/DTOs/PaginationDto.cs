namespace MD_Tech.DTOs
{
    public class PaginationDto
    {
        public int Size { get; set; } = 25;

        public int Page { get; set; } = 0;

        /// <summary>
        /// Formato esperado = nombre-desc o nombre-asc o nombre
        /// </summary>
        public string? OrderBy { get; set; }
    }
}
