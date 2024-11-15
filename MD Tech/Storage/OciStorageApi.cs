using Oci.Common.Auth;
using Oci.ObjectstorageService;
using Oci.ObjectstorageService.Requests;

namespace MD_Tech.Storage
{
    public class OciStorageApi : IStorageApi
    {
        private readonly ConfigFileAuthenticationDetailsProvider Provider;
        private readonly LogsApi<OciStorageApi> logger;
        private string BucketName;
        private string Namespace;

        public OciStorageApi(IConfiguration configuration, LogsApi<OciStorageApi> logger)
        {
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "oci", "oci-mdtech.config");
            Environment.SetEnvironmentVariable("OCI_CONFIG_FILE", baseDirectory);
            Environment.SetEnvironmentVariable("OCI_SDK_DEFAULT_RETRY_ENABLED", "true");
            Provider = new("DEFAULT");
            this.logger = logger;
            BucketName = configuration["ObjectStorage:OCI:BucketName"] ?? string.Empty;
            Namespace = configuration["ObjectStorage:OCI:Namespace"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(BucketName) || string.IsNullOrWhiteSpace(Namespace))
            {
                throw new Exception("ObjectStorageOci no pudo ser configurado por falta de datos. Revisar o usar otra implementación");
            }
        }

        public async Task<StorageApiDto?> GetObjectAsync(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                logger.Advertencia("Se ha rechazado búsqueda de objeto en OCI por nombre inválido");
                return null;
            }
            using var client = new ObjectStorageClient(Provider, new());
            var getObjectRequest = new GetObjectRequest()
            {
                BucketName = BucketName,
                NamespaceName = Namespace,
                ObjectName = objectName,
            };

            try
            {
                var response = await client.GetObject(getObjectRequest);
                logger.Informacion($"Obtener objeto exitoso ETAG: {response.ETag}");
                return new StorageApiDto()
                {
                    Name = objectName,
                    Type = response.ContentType,
                    Url = new Uri(client.GetEndpoint(), $"/n/{Namespace}/b/{BucketName}/o/prueba"),
                    Status = true,
                    Stream = response.InputStream,
                };
            } catch (Exception ex)
            {
                logger.Excepciones(ex, "Ha ocurrido un error al obtener de OCI");
            }
            return null;
        }

        public async Task<StorageApiDto?> PutObjectAsync(StorageApiDto storageDto)
        {
            using var client = new ObjectStorageClient(Provider, new());
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = BucketName,
                NamespaceName = Namespace,
                ObjectName = storageDto.Name,
                PutObjectBody = storageDto.Stream,
                ContentType = storageDto.Type,
            };

            try
            {
                var response = await client.PutObject(putObjectRequest);
                logger.Informacion($"Subida de objecto a OCI exitoso ETAG: {response.ETag}");
                return new StorageApiDto()
                {
                    Status = true,
                    Name = storageDto.Name,
                    Type = storageDto.Type,
                    Stream = Stream.Null,
                    Url = new Uri(client.GetEndpoint(), $"/n/{Namespace}/b/{BucketName}/o/{storageDto.Name}")
                };
            }
            catch (Exception ex)
            {
                logger.Excepciones(ex, "Ha ocurrido un error al guardar un objecto en OCI");
            }
            return null;
        }

        public async Task<bool> DeleteObjectAsync(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                logger.Advertencia("Se ha rechazado eliminación de objeto en OCI por nombre inválido");
                return false;
            }
            using var client = new ObjectStorageClient(Provider, new ());
            var deleteObjectRequest = new DeleteObjectRequest()
            {
                BucketName = BucketName,
                NamespaceName = Namespace,
                ObjectName = objectName,
            };

            try
            {
                var response = await client.DeleteObject(deleteObjectRequest);
                return response.httpResponseMessage.IsSuccessStatusCode;
            } catch (Exception ex)
            {
                logger.Excepciones(ex, "Ha ocurrido un error al eliminar en OCI");
            }
            return false;
        }
    }
}
