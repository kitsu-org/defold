using Microsoft.EntityFrameworkCore;

namespace Defold.Models;

[PrimaryKey(nameof(Bucket), nameof(Key))]
public class UploadedFile
{
    public string Bucket { get; set; }
    public string Key { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadedAt { get; set; }

    public ICollection<FileChunk> Chunks { get; } = new List<FileChunk>();

}