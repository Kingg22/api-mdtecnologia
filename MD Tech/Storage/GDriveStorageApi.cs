using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;

namespace MD_Tech.Storage
{
    public class GDriveStorageApi : IStorageApi
    {
        private DriveService driveService;
        private LogsApi logger;

        public GDriveStorageApi(IConfiguration configuration)
        {
            var appName = configuration["Google-Drive:appName"];
            logger = new LogsApi(GetType());
            _ = GetDriveServiceAsync(appName);
            if (driveService == null)
            {
                throw new Exception("No se ha podido configurar Google Drive service");
            }
        }

        private async Task<DriveService> GetDriveServiceAsync(string? appName)
        {
            UserCredential credential;
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                (await GoogleClientSecrets.FromFileAsync("drive/credentials.json")).Secrets,
                [DriveService.Scope.Drive],
                "user",
                CancellationToken.None,
                new FileDataStore("drive.api.auth.store")
                );
            var service = new DriveService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = appName,
            });
            driveService = service;
            return service;
        }

        public async Task<string> CreateFolder(string folderName)
        {
            var driveFolder = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
            };
            var file = await driveService.Files.Create(driveFolder).ExecuteAsync();
            logger.Informacion($"Se ha creado un nuevo folder en GDrive ETAG: {file.ETag}");
            return file.Id;
        }

        public Task<object?> GetObjectAsync(string objectName)
        {
            throw new NotImplementedException();
        }

        public async Task<Uri?> PutObjectAsync(Stream objectToSave, string objectName, string type, string[]? folders)
        {
            var driveFile = new Google.Apis.Drive.v3.Data.File()
            {
                Name = objectName,
                MimeType = type,
                Parents = folders,
            };
            var request = driveService.Files.Create(driveFile, objectToSave, type);
            var response = await request.UploadAsync();
            if (response.Status != Google.Apis.Upload.UploadStatus.Completed) throw response.Exception;
            Uri.TryCreate(request.Path, UriKind.Absolute, out Uri? responseUrl);
            return responseUrl;
        }

        public async Task<bool> DeleteObjectAsync(string objectName)
        {
            var result = driveService.Files.List();
            result.Q = $"name contains {objectName}";
            var lista = await result.ExecuteAsync();
            if (lista.Files.Count > 0)
            {
                if (lista.Files.Count > 1)
                {
                    logger.Advertencia("Se ha decidido no eliminar ningun archivo de GDrive ya que la lista es mayor a 1 elemento");
                }
                else
                {
                    var item = lista.Files.FirstOrDefault();
                    if (item != null)
                    {
                        await driveService.Files.Delete(item.Id).ExecuteAsync();
                        return true;
                    }
                }
            }
            else
            {
                logger.Errores($"No se ha encontrado archivos en GDrive con el nombre: {objectName}");
            }
            return false;
        }
    }
}
