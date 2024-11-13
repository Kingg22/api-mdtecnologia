using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MD_Tech.Storage
{
    public interface IStorageApi
    {
        Task<StorageApiDto?> PutObjectAsync(StorageApiDto storageApiDto);

        Task<bool> DeleteObjectAsync(string objectName);

        Task<StorageApiDto?> GetObjectAsync(string objectName);
    }

    public class StorageApiDto
    {
        public bool Status { get; set; } = false;
        public Uri? Url { get; set; }
        public string Type { get; set; } = "application/octet-stream";
        public required string Name { get; set; }
        public required Stream Stream { get; set; }
    }
}
