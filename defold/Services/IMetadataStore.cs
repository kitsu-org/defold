using Defold.Models;

namespace Defold.Services;

public interface IMetadataStore
{
    /// <summary>
    /// Add metadata on a file and its' chunks
    /// </summary>
    /// <param name="file">File to store</param>
    Task AddFile(Models.UploadedFile file);

    /// <summary>
    /// Fetch list of files in a bucket
    /// </summary>
    /// <param name="bucket">Bucket to fetch files for</param>
    /// <param name="prefix">Prefix for keys to return</param>
    Task<List<UploadedFile>> GetFiles(string bucket, string? prefix = null);
    
}