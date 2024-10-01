using Defold.Models;
using Defold.Services;
using S3ServerLibrary;
using S3ServerLibrary.S3Objects;

namespace Defold;

public class DefoldS3Server
{
    private ILogger Logger { get; set; }
    private IMetadataStore MetadataStore { get; set; }

    public DefoldS3Server(ILogger<DefoldS3Server> logger, IMetadataStore metadataStore)
    {
        Logger = logger;
        MetadataStore = metadataStore;
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
    
    private Task ObjectWrite(S3Context arg)
    {
        var uploadedFile = new UploadedFile()
        {
            Bucket = arg.Request.Bucket,
            ContentType = arg.Request.ContentType,
            Key = arg.Request.Key,
            UploadedAt = DateTime.Now
        };
        
    }
}