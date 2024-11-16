using MD_Tech.Models;
using Microsoft.EntityFrameworkCore;

namespace MD_Tech.Context;

public partial class MdtecnologiaContext : DbContext
{
    public MdtecnologiaContext(DbContextOptions<MdtecnologiaContext> options) : base(options) { }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<ContactoProveedor> ContactosProveedores { get; set; }

    public virtual DbSet<DetalleVenta> DetallesVentas { get; set; }

    public virtual DbSet<DireccionesCliente> DireccionesClientes { get; set; }

    public virtual DbSet<Direccion> Direcciones { get; set; }

    public virtual DbSet<HistorialProducto> HistorialProductos { get; set; }

    public virtual DbSet<ImagenesProducto> ImagenesProductos { get; set; }

    public virtual DbSet<OrdenesCompraProveedor> OrdenesCompraProveedores { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductosProveedor> ProductosProveedores { get; set; }

    public virtual DbSet<Proveedor> Proveedores { get; set; }

    public virtual DbSet<Provincia> Provincias { get; set; }

    public virtual DbSet<Trabajador> Trabajadores { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categorias_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.CategoriaPadreNavigation).WithMany(p => p.InverseCategoriaPadreNavigation)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("categorias_categoria_padre_fkey");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("clientes_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.UsuarioNavigation).WithOne(p => p.Cliente)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("clientes_usuario_fkey");

            entity.HasMany(d => d.Direcciones).WithMany(p => p.Clientes).UsingEntity<DireccionesCliente>();
        });

        modelBuilder.Entity<ContactoProveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contacto_proveedor_pkey");

            entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.ContactoProveedors)
                .HasConstraintName("contacto_proveedor_proveedor_fkey");
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalles_venta_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Descuento).HasDefaultValueSql("0.00");
        });

        modelBuilder.Entity<DireccionesCliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("direccion_cliente_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.ClienteNavigation).WithMany(p => p.DireccionClientes)
                .HasConstraintName("direccion_cliente_cliente_fkey");

            entity.HasOne(d => d.DireccionNavigation).WithMany(p => p.DireccionClientes)
                .HasConstraintName("direccion_cliente_direccion_fkey");
        });

        modelBuilder.Entity<Direccion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("direcciones_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Provincia).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ProvinciaNavigation).WithMany(p => p.Direcciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("direcciones_provincia_fkey");

            entity.HasMany(d => d.Clientes).WithMany(p => p.Direcciones).UsingEntity<DireccionesCliente>();
        });

        modelBuilder.Entity<HistorialProducto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("historial_productos_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.FechaCambio).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.ProductoNavigation).WithMany(p => p.HistorialProductos)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("historial_productos_producto_fkey");

            entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.HistorialProductos)
                .HasConstraintName("historial_productos_proveedor_fkey");
        });

        modelBuilder.Entity<ImagenesProducto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("imagenes_productos_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.ProductoNavigation).WithMany(p => p.ImagenesProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("imagenes_productos_producto_fkey");
        });

        modelBuilder.Entity<OrdenesCompraProveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ordenes_compra_proveedor_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.OrdenesCompraProveedors)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("ordenes_compra_proveedor_proveedor_fkey");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productos_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.CategoriaNavigation).WithMany(p => p.Productos)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("productos_categoria_fkey");
        });

        modelBuilder.Entity<ProductosProveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productos_proveedores_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.FechaActualizado).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.ProductoNavigation).WithMany(p => p.ProductosProveedores)
                .HasConstraintName("productos_proveedores_producto_fkey");

            entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.ProductosProveedores)
                .HasConstraintName("productos_proveedores_proveedor_fkey");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proveedores_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.DireccionNavigation).WithMany(p => p.Proveedores)
                .HasConstraintName("proveedores_direccion_fkey");
        });

        modelBuilder.Entity<Provincia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("provincias_pkey");
        });

        modelBuilder.Entity<Trabajador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trabajadores_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Estado).HasDefaultValue(true);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuarios_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Disabled).HasDefaultValue(false);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ventas_pkey");

            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Descuento).HasDefaultValueSql("0.00");
            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
