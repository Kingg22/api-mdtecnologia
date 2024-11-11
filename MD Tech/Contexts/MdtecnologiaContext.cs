using MD_Tech.Models;
using Microsoft.EntityFrameworkCore;

namespace MD_Tech.Contexts;

public partial class MdtecnologiaContext : DbContext
{
    public MdtecnologiaContext(DbContextOptions<MdtecnologiaContext> options) : base(options) { }

    public virtual DbSet<Categorias> Categorias { get; set; }

    public virtual DbSet<Clientes> Clientes { get; set; }

    public virtual DbSet<ContactoProveedor> ContactoProveedors { get; set; }

    public virtual DbSet<DetallesVentas> DetallesVenta { get; set; }

    public virtual DbSet<DireccionesClientes> DireccionClientes { get; set; }

    public virtual DbSet<Direcciones> Direcciones { get; set; }

    public virtual DbSet<HistorialProducto> HistorialProductos { get; set; }

    public virtual DbSet<ImagenesProducto> ImagenesProductos { get; set; }

    public virtual DbSet<OrdenesCompraProveedor> OrdenesCompraProveedors { get; set; }

    public virtual DbSet<Productos> Productos { get; set; }

    public virtual DbSet<ProductosProveedor> ProductosProveedores { get; set; }

    public virtual DbSet<Proveedores> Proveedores { get; set; }

    public virtual DbSet<Provincia> Provincias { get; set; }

    public virtual DbSet<Trabajadores> Trabajadores { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<Ventas> Ventas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categorias_pkey");

            entity.ToTable("categorias");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CategoriaPadre).HasColumnName("categoria_padre");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");

            entity.HasOne(d => d.CategoriaPadreNavigation).WithMany(p => p.InverseCategoriaPadreNavigation)
                .HasForeignKey(d => d.CategoriaPadre)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("categorias_categoria_padre_fkey");
        });

        modelBuilder.Entity<Clientes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("clientes_pkey");

            entity.ToTable("clientes");

            entity.HasIndex(e => e.Usuario, "clientes_usuario_key").IsUnique();

            entity.HasIndex(e => e.Correo, "clientes_correo_key").IsUnique();

            entity.HasIndex(e => e.Telefono, "clientes_telefono_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Correo)
                .HasMaxLength(255)
                .HasColumnName("correo");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .HasColumnName("telefono");
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
            entity.Property(e => e.Usuario).HasColumnName("usuario");

            entity.HasOne(d => d.UsuarioNavigation).WithOne(p => p.Cliente)
                .HasForeignKey<Clientes>(c => c.Usuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("clientes_usuario_fkey");
        });

        modelBuilder.Entity<ContactoProveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contacto_proveedor_pkey");

            entity.ToTable("contacto_proveedor");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Correo)
                .HasMaxLength(255)
                .HasColumnName("correo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Proveedor).HasColumnName("proveedor");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .HasColumnName("telefono");

            entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.ContactoProveedors)
                .HasForeignKey(d => d.Proveedor)
                .HasConstraintName("contacto_proveedor_proveedor_fkey");
        });

        modelBuilder.Entity<DetallesVentas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalles_venta_pkey");

            entity.ToTable("detalles_venta");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Descuento)
                .HasDefaultValueSql("'B/.0.00'::money")
                .HasColumnType("money")
                .HasColumnName("descuento");
            entity.Property(e => e.Impuesto)
                .HasColumnType("money")
                .HasColumnName("impuesto");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("money")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.Producto).HasColumnName("producto");
            entity.Property(e => e.Subtotal)
                .HasColumnType("money")
                .HasColumnName("subtotal");
            entity.Property(e => e.Total)
                .HasColumnType("money")
                .HasColumnName("total");
            entity.Property(e => e.Venta).HasColumnName("venta");

            entity.HasOne(d => d.ProductoNavigation).WithMany(p => p.DetallesVenta)
                .HasForeignKey(d => d.Producto)
                .HasConstraintName("detalles_venta_producto_fkey");

            entity.HasOne(d => d.VentaNavigation).WithMany(p => p.DetallesVenta)
                .HasForeignKey(d => d.Venta)
                .HasConstraintName("detalles_venta_venta_fkey");
        });

        modelBuilder.Entity<DireccionesClientes>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("direccion_cliente");

            entity.Property(e => e.Cliente).HasColumnName("cliente");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Direccion).HasColumnName("direccion");

            entity.HasOne(d => d.ClienteNavigation).WithMany()
                .HasForeignKey(d => d.Cliente)
                .HasConstraintName("direccion_cliente_cliente_fkey");

            entity.HasOne(d => d.DireccionNavigation).WithMany()
                .HasForeignKey(d => d.Direccion)
                .HasConstraintName("direccion_cliente_direccion_fkey");
        });

        modelBuilder.Entity<Direcciones>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("direcciones_pkey");

            entity.ToTable("direcciones");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Provincia)
                .ValueGeneratedOnAdd()
                .HasColumnName("provincia");

            entity.HasOne(d => d.ProvinciaNavigation).WithMany(p => p.Direcciones)
                .HasForeignKey(d => d.Provincia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("direcciones_provincia_fkey");
        });

        modelBuilder.Entity<HistorialProducto>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("historial_productos");

            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha_cambio");
            entity.Property(e => e.PrecioBaseAnterior)
                .HasColumnType("money")
                .HasColumnName("precio_base_anterior");
            entity.Property(e => e.PrecioTotalAnterior)
                .HasColumnType("money")
                .HasColumnName("precio_total_anterior");
            entity.Property(e => e.Producto).HasColumnName("producto");

            entity.HasOne(d => d.ProductoNavigation).WithMany()
                .HasForeignKey(d => d.Producto)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("historial_productos_producto_fkey");
        });

        modelBuilder.Entity<ImagenesProducto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("imagenes_productos_pkey");

            entity.ToTable("imagenes_productos");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Producto).HasColumnName("producto");
            entity.Property(e => e.Url).HasColumnName("url");

            entity.HasOne(d => d.ProductoNavigation).WithMany(p => p.ImagenesProductos)
                .HasForeignKey(d => d.Producto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("imagenes_productos_producto_fkey");
        });

        modelBuilder.Entity<OrdenesCompraProveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ordenes_compra_proveedor_pkey");

            entity.ToTable("ordenes_compra_proveedor");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.FechaEstimadaEntrega).HasColumnName("fecha_estimada_entrega");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.Proveedor).HasColumnName("proveedor");

            entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.OrdenesCompraProveedores)
                .HasForeignKey(d => d.Proveedor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("ordenes_compra_proveedor_proveedor_fkey");
        });

        modelBuilder.Entity<Productos>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productos_pkey");

            entity.ToTable("productos");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Categoria).HasColumnName("categoria");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Marca).HasColumnName("marca");
            entity.Property(e => e.Nombre).HasColumnName("nombre");

            entity.HasOne(d => d.CategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.Categoria)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("productos_categoria_fkey");
        });

        modelBuilder.Entity<ProductosProveedor>(entity =>
        {
            entity.HasKey(e => new { e.Producto, e.Proveedor }).HasName("productos_proveedores_pkey");

            entity.ToTable("productos_proveedores");

            entity.Property(e => e.Producto).HasColumnName("producto");
            entity.Property(e => e.Proveedor).HasColumnName("proveedor");
            entity.Property(e => e.FechaActualizado)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("fecha_actualizado");
            entity.Property(e => e.Impuesto)
                .HasColumnType("money")
                .HasColumnName("impuesto");
            entity.Property(e => e.Precio)
                .HasColumnType("money")
                .HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.Total)
                .HasColumnType("money")
                .HasColumnName("total");

            entity.HasOne(d => d.ProductoNavigation).WithMany(p => p.ProductosProveedores)
                .HasForeignKey(d => d.Producto)
                .HasConstraintName("productos_proveedores_producto_fkey");

            entity.HasOne(d => d.ProveedorNavigation).WithMany(p => p.ProductosProveedores)
                .HasForeignKey(d => d.Proveedor)
                .HasConstraintName("productos_proveedores_proveedor_fkey");
        });

        modelBuilder.Entity<Proveedores>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proveedores_pkey");

            entity.ToTable("proveedores");

            entity.HasIndex(e => e.Correo, "proveedores_correo_key").IsUnique();

            entity.HasIndex(e => e.Telefono, "proveedores_telefono_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Correo)
                .HasMaxLength(255)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .HasColumnName("telefono");

            entity.HasOne(d => d.DireccionNavigation).WithMany(p => p.Proveedores)
                .HasForeignKey(d => d.Direccion)
                .HasConstraintName("proveedores_direccion_fkey");
        });

        modelBuilder.Entity<Provincia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("provincias_pkey");

            entity.ToTable("provincias");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Trabajadores>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trabajadores_pkey");

            entity.ToTable("trabajadores");

            entity.HasIndex(e => e.Correo, "trabajadores_correo_key").IsUnique();

            entity.HasIndex(e => e.Telefono, "trabajadores_telefono_key").IsUnique();

            entity.HasIndex(e => e.Usuario, "trabajadores_usuario_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Cargo)
                .HasMaxLength(100)
                .HasColumnName("cargo");
            entity.Property(e => e.Correo)
                .HasMaxLength(255)
                .HasColumnName("correo");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Salario)
                .HasColumnType("money")
                .HasColumnName("salario");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .HasColumnName("telefono");
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
            entity.Property(e => e.Usuario).HasColumnName("usuario");

            entity.HasOne(d => d.UsuarioNavigation).WithOne(p => p.Trabajador)
                .HasForeignKey<Trabajadores>(d => d.Usuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("trabajadores_usuario_fkey");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuarios_pkey");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Username, "usuarios_username_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.Disabled)
                .HasDefaultValue(false)
                .HasColumnName("disabled");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .HasColumnName("rol");
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Ventas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ventas_pkey");

            entity.ToTable("ventas");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CantidadTotalProductos).HasColumnName("cantidad_total_productos");
            entity.Property(e => e.Cliente).HasColumnName("cliente");
            entity.Property(e => e.Descuento)
                .HasDefaultValueSql("'B/.0.00'::money")
                .HasColumnType("money")
                .HasColumnName("descuento");
            entity.Property(e => e.DireccionEntrega).HasColumnName("direccion_entrega");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("fecha");
            entity.Property(e => e.Impuesto)
                .HasColumnType("money")
                .HasColumnName("impuesto");
            entity.Property(e => e.Subtotal)
                .HasColumnType("money")
                .HasColumnName("subtotal");
            entity.Property(e => e.Total)
                .HasColumnType("money")
                .HasColumnName("total");

            entity.HasOne(d => d.ClienteNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.Cliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ventas_cliente_fkey");

            entity.HasOne(d => d.DireccionEntregaNavigation).WithMany(p => p.Ventas)
                .HasForeignKey(d => d.DireccionEntrega)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("ventas_direccion_entrega_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
