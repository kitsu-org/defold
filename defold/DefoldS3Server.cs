using System.Security.Cryptography;
using System.Text;
using Defold.Models;
using Defold.Services;
using Microsoft.Extensions.Options;
using S3ServerLibrary;
using S3ServerLibrary.S3Objects;

namespace Defold;

public class DefoldS3Server(
    ILogger<DefoldS3Server> logger,
    IMetadataStore metadataStore,
    IOptions<DeduplicationOptions> dedupOptions,
    IFileStore fileStore)
{
    private ILogger Logger { get; } = logger;
    private IMetadataStore MetadataStore { get; } = metadataStore;
    private IFileStore FileStore { get; } = fileStore;
    private IOptions<DeduplicationOptions> DedupOptions { get; } = dedupOptions;

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

            Logger.ChunkReadFromRequest(chunkIndex);
            
            // spin up a bunch of threads (limited by the .NET thread pool) to calculate
            // the hash (within the FileStore) for each file chunk and turn it into a
            // FileChunk object we attach to the UploadedFile we created above
            tasks.Add(Task.Run<FileChunk>(async () =>
            {
                Logger.ChunkProcessingThreadStarted(chunkIndex);
                var hash = await FileStore.StoreChunk(bytes);
                return new FileChunk()
                {
                    Bucket = arg.Request.Bucket,
                    Key = arg.Request.Key,
                    ChunkIndex = chunkIndex,
                    ChunkHash = hash
                };
            }));
        }
        Logger.WaitingForChunkThreads();
        await Task.WhenAll(tasks);
        Logger.ChunkThreadsFinished();
        var chunks = tasks.Select(t => t.Result);
        foreach (var chunk in chunks) uploadedFile.Chunks.Add(chunk);
        await MetadataStore.AddFile(uploadedFile);
        Logger.FileStored(arg.Request.Key);
    }
}

public static partial class DefoldS3ServerLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Chunk {chunkIndex} read from request")]
    public static partial void ChunkReadFromRequest(this ILogger logger, int chunkIndex);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Thread for chunk {chunkIndex} started")]
    public static partial void ChunkProcessingThreadStarted(this ILogger logger, int chunkIndex);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Waiting on all chunk threads to finish")]
    public static partial void WaitingForChunkThreads(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "All chunk threads finished")]
    public static partial void ChunkThreadsFinished(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "File {key} uploaded and all metadata stored")]
    public static partial void FileStored(this ILogger logger, string key);
}