using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Defold.Models;

[PrimaryKey(nameof(Bucket), nameof(Key), nameof(ChunkIndex))]
[Index(nameof(Bucket), nameof(Key))]
public class FileChunk
{
    public string Bucket { get; set; }
    public string Key { get; set; }
    public long ChunkIndex { get; set; }
    public string ChunkHash { get; set; }

    [ForeignKey("Bucket, Key")]
    public UploadedFile File { get; set; }
}