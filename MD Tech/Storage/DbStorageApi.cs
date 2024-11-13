using MD_Tech.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MD_Tech.Storage
{
    [Obsolete("Esta implementación no es recomendada, considere usar otra", false)]
    public class DbStorageApi : IStorageApi
    {
        private readonly ImagenesContext context;
        private readonly LogsApi logger;
        private readonly Uri urlBase;
        private static bool created = false;

        public DbStorageApi(DbContextOptions<MdtecnologiaContext> contextOptions) 
        {
            context = new ImagenesContext(contextOptions);
            logger = new LogsApi(GetType());
            urlBase = new Uri("http://localhost:5294/api");
            if (!created)
            {
                _ = CreateTable();
                created = true;
            }
        }

        private async Task CreateTable()
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

        public async Task<StorageApiDto?> GetObjectAsync(string objectName)
        {
            var result = await context.Imagenes.Where(i => EF.Functions.ILike(i.ImagenName, $"%{objectName}%")).FirstOrDefaultAsync();
            return result == null ? null : new StorageApiDto()
            {
                Name = result.ImagenName,
                Type = result.Type,
                Stream = new MemoryStream(result.ImagenData), 
                Status = true,
                Url = null,
            };
        }

        public async Task<StorageApiDto?> PutObjectAsync(StorageApiDto storageApiDto)
        {
            await using var ms = new MemoryStream();
            await storageApiDto.Stream.CopyToAsync(ms);
            var imagenData = ms.ToArray();

            var imagen = new Imagenes()
            {
                Id = Guid.NewGuid(),
                ImagenName = storageApiDto.Name,
                Type = storageApiDto.Type,
                ImagenData = imagenData
            };
            await context.AddAsync(imagen);
            await context.SaveChangesAsync();
            if (Uri.TryCreate(urlBase, $"/Imagenes/{imagen.ImagenName}", out Uri? newUri))
            {
                storageApiDto.Url = newUri;
                storageApiDto.Status = true;
                storageApiDto.Stream = Stream.Null;
                return storageApiDto;
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

    public partial class Imagenes
    {
        public Guid Id { get; set; }

        public string ImagenName { get; set; } = null!;

        public string Type { get; set; } = null!;

        public byte[] ImagenData { get; set; } = null!;
    }
}
