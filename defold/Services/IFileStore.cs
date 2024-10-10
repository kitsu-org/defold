namespace Defold.Services;

public interface IFileStore
{
    /// <summary>
    /// Store a chunk and return its hash
    /// </summary>
    /// <param name="chunk">Chunk to store</param>
    /// <returns>Hash of the chunk</returns>
    Task<string> StoreChunk(byte[] chunk);
}