using System.Security.Cryptography;
using System.Text;
using Defold.Models;
using Defold.Services;
using Microsoft.Extensions.Options;
using S3ServerLibrary;
using S3ServerLibrary.S3Objects;

namespace Defold;

public class DefoldS3Server
{
    private ILogger Logger { get; }
    private IMetadataStore MetadataStore { get; }
    private IOptions<DeduplicationOptions> DedupOptions { get; }

    public DefoldS3Server(
        ILogger<DefoldS3Server> logger, 
        IMetadataStore metadataStore,
        IOptions<DeduplicationOptions> dedupOptions)
    {
        Logger = logger;
        MetadataStore = metadataStore;
        DedupOptions = dedupOptions;
    }
    
    public void SetupCallbacks(S3Server server)
    {
        server.Bucket.Read += BucketRead;
        server.Object.Write += ObjectWrite;
    }

    private async Task<ListBucketResult> BucketRead(S3Context ctx)
    {
        var result = new ListBucketResult(
            ctx.Request.Bucket,
            new List<ObjectMetadata>(),
            0,
            ctx.Request.MaxKeys,
            ctx.Request.Prefix,
            ctx.Request.Marker,
            ctx.Request.Delimiter,
            false,
            null,
            null,
            ctx.Request.Region
        );
        return result;
    }
    
    private async Task ObjectWrite(S3Context arg)
    {
        var uploadedFile = new UploadedFile()
        {
            Bucket = arg.Request.Bucket,
            ContentType = arg.Request.ContentType,
            Key = arg.Request.Key,
            UploadedAt = DateTime.UtcNow
        };
        var tasks = new List<Task<FileChunk>>();
        var totalSize = arg.Request.ContentLength;
        var chunkSize = DedupOptions.Value.ChunkSize;
        var numChunks = (int)Math.Ceiling(totalSize / (float)chunkSize);
        var stream = arg.Request.Data;
        for (var i = 0; i < numChunks; i++)
        {
            var chunkIndex = i;
            var expectedSize = chunkSize;
            
            // if this is the last chunk, we expect it to be a bit smaller, ie. if we have a chunk size of 64 but
            // only 70 bytes for example
            if (i == (numChunks - 1))
            {
                expectedSize = (int)(totalSize % chunkSize);
            }
            
            // we read bytes outside the task as otherwise we risk reading the data out of order
            // (some stream implementations are not thread-safe, and we can't seek on the request
            // stream, either, before reading)
            var bytes = new byte[expectedSize];
            await stream.ReadExactlyAsync(bytes, 0, expectedSize);
            
            // spin up a bunch of threads (limited by the .NET thread pool) to calculate
            // the hash for each file chunk and turn it into a FileChunk object we attach to the
            // UploadedFile we created above
            tasks.Add(Task.Run<FileChunk>(() =>
            {
                var hash = SHA256.HashData(bytes);
                var sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                var hashString = sb.ToString();
                return new FileChunk()
                {
                    Bucket = arg.Request.Bucket,
                    Key = arg.Request.Key,
                    ChunkIndex = chunkIndex,
                    ChunkHash = hashString
                };
            }));
        }
        await Task.WhenAll(tasks);
        var chunks = tasks.Select(t => t.Result);
        foreach (var chunk in chunks) uploadedFile.Chunks.Add(chunk);
        await MetadataStore.AddFile(uploadedFile);
    }
}