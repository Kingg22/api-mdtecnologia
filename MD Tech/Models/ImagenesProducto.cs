namespace MD_Tech.Models
{
    public partial class ImagenesProducto
    {
        public Guid Id { get; set; }

        public string? Descripcion { get; set; }

        public string Url { get; set; } = null!;

        public Guid Producto { get; set; }

        public virtual Productos ProductoNavigation { get; set; } = null!;
    }

}
