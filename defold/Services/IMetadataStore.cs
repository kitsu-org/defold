namespace Defold.Services;

public interface IMetadataStore
{
    /// <summary>
    /// Add metadata on a file and its' chunks
    /// </summary>
    /// <param name="file">File to store</param>
    Task AddFile(Models.UploadedFile file);
}