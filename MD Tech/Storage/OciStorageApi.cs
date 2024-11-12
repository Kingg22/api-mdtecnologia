﻿using Oci.Common.Auth;
using Oci.ObjectstorageService;
using Oci.ObjectstorageService.Requests;

namespace MD_Tech.Storage
{
    public class OciStorageApi : IStorageApi
    {
        private readonly ConfigFileAuthenticationDetailsProvider Provider;
        private readonly LogsApi logs;
        private string BucketName;
        private string Namespace;

        public OciStorageApi(IConfiguration configuration)
        {
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "oci", "oci-mdtech.config");
            Environment.SetEnvironmentVariable("OCI_CONFIG_FILE", baseDirectory);
            Environment.SetEnvironmentVariable("OCI_SDK_DEFAULT_RETRY_ENABLED", "true");
            Provider = new("DEFAULT");
            logs = new LogsApi(GetType());
            BucketName = configuration["ObjectStorage:OCI:BucketName"] ?? string.Empty;
            Namespace = configuration["ObjectStorage:OCI:Namespace"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(BucketName) || string.IsNullOrWhiteSpace(Namespace))
            {
                throw new Exception("ObjectStorageOci no pudo ser configurado por falta de datos. Revisar o usar otra implementación");
            }
        }

        public async Task<object?> GetObjectAsync(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                logs.Advertencia("Se ha rechazado búsqueda de objeto en OCI por nombre inválido");
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
                logs.Informacion($"Obtener objeto exitoso ETAG: {response.ETag}");
                return response;
            } catch (Exception ex)
            {
                logs.Excepciones(ex, "Ha ocurrido un error al obtener de OCI");
            }
            return null;
        }

        public async Task<Uri?> PutObjectAsync(Stream objectToSave, string objectName, string type)
        {
            using var client = new ObjectStorageClient(Provider, new());
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = BucketName,
                NamespaceName = Namespace,
                ObjectName = objectName,
                PutObjectBody = objectToSave,
                ContentType = type,
            };

            try
            {
                var response = await client.PutObject(putObjectRequest);
                logs.Informacion($"Subida de objecto a OCI exitoso ETAG: {response.ETag}");
                return new Uri(client.GetEndpoint(), $"/n/{Namespace}/b/{BucketName}/o/{objectName}");
            }
            catch (Exception ex)
            {
                logs.Excepciones(ex, "Ha ocurrido un error al guardar un objecto en OCI");
            }
            return null;
        }

        public async Task<bool> DeleteObjectAsync(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                logs.Advertencia("Se ha rechazado eliminación de objeto en OCI por nombre inválido");
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
                logs.Excepciones(ex, "Ha ocurrido un error al eliminar en OCI");
            }
            return false;
        }
    }
}
