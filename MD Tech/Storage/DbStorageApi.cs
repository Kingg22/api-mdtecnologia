using MD_Tech.Contexts;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MD_Tech.Storage
{
    public class DbStorageApi : IStorageApi
    {
        private readonly ImagenesContext context;
        private readonly LogsApi logger;
        private static bool created = false;

        public DbStorageApi(DbContextOptions<MdtecnologiaContext> contextOptions) 
        {
            context = new ImagenesContext(contextOptions);
            logger = new LogsApi(GetType());
            if (!created)
            {
                CreateTable();
                created = true;
            }
        }

        private async void CreateTable()
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var instruccion = "CREATE TABLE IF NOT EXISTS imagenes (" +
                    "id UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(), " +
                    "imagen_name TEXT NOT NULL, " +
                    "type VARCHAR(50) NOT NULL DEFAULT 'image/png', " +
                    "imagen_data BYTEA NOT NULL" +
                    ")";
                await context.Database.ExecuteSqlRawAsync(instruccion);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Error al configurar tablas para DbStorage");
                await transaction.RollbackAsync();
                throw new Exception("DbStorage no pudo configurar correctamente las tablas en la BD, intente nuevamente");
            }
        }

        public async Task<object?> GetObjectAsync(string objectName)
        {
            var result = await context.Imagenes.Where(i => EF.Functions.ILike(i.ImagenName, $"%{objectName}%")).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }
            return result;
        }

        public async Task<Uri?> PutObjectAsync(Stream objectToSave, string objectName, string type)
        {
            await using var ms = new MemoryStream();
            await objectToSave.CopyToAsync(ms);
            var imagenData = ms.ToArray();

            var imagen = new Imagenes()
            {
                Id = Guid.NewGuid(),
                ImagenName = objectName,
                Type = type,
                ImagenData = imagenData
            };
            await context.AddAsync(imagen);
            await context.SaveChangesAsync();
            if (Uri.TryCreate($"http://localhost:5294/api/Imagenes/{imagen.ImagenName}", UriKind.Absolute, out Uri? newUri))
            {
                return newUri;
            }
            else
            {
                logger.Errores("No se pudo crear la uri para el recurso creado");
            }
            return null;
        }

        public async Task<bool> DeleteObjectAsync(string objectName)
        {
            var result = await context.Imagenes.Where(i => EF.Functions.ILike(i.ImagenName, objectName)).FirstOrDefaultAsync();
            if (result == null)
            {
                logger.Errores("No se pudo encontrar la imagen en la base de datos");
            } 
            else
            {
                context.Remove(result);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }

    partial class ImagenesContext : MdtecnologiaContext
    {
        public ImagenesContext(DbContextOptions<MdtecnologiaContext> options) : base(options) { }

        public virtual DbSet<Imagenes> Imagenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Imagenes>(entity =>
            {
                entity.ToTable("imagenes");

                entity.HasKey(i => i.Id).HasName("imagenes_pkey");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasColumnName("id");

                entity.Property(e => e.ImagenName)
                    .IsRequired()
                    .HasColumnName("imagen_name");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("type")
                    .HasDefaultValue("image/png");

                entity.Property(entity => entity.ImagenData)
                    .IsRequired()
                    .HasColumnName("imagen_data");
            }
            );
        }
    }

    [Table("imagenes")]
    public partial class Imagenes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string ImagenName { get; set; } = null!;

        [StringLength(50, MinimumLength = 1)]
        public string Type { get; set; } = null!;

        public byte[] ImagenData { get; set; } = null!;
    }
}
