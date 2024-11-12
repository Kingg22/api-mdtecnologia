namespace MD_Tech.Storage
{
    public interface IStorageApi
    {
        Task<Uri?> PutObjectAsync(Stream objectToSave, string objectName, string type);

        Task<bool> DeleteObjectAsync(string objectName);

        Task<object?> GetObjectAsync(string objectName);
    }
}
