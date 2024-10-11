using Microsoft.EntityFrameworkCore;
using S3ServerLibrary.S3Objects;

namespace Defold.Models;

[PrimaryKey(nameof(Bucket), nameof(Key))]
public class UploadedFile
{
    public string Bucket { get; set; }
    public string Key { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadedAt { get; set; }

    public ICollection<FileChunk> Chunks { get; } = new List<FileChunk>();

    public ObjectMetadata ToS3Object()
    {
        return new ObjectMetadata()
        {
            ContentType = ContentType,
            LastModified = UploadedAt,
            Owner = null,
            Key = Key,
            Size = 0,
            StorageClass = StorageClassEnum.STANDARD,
            ETag = ""
        };
    }
}