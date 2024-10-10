using System.Security.Cryptography;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Defold.Utility;
using Microsoft.Extensions.Options;

namespace Defold.Services;

public class S3FileStore(IOptions<S3ClientOptions> options, ILogger<S3FileStore> logger) : IFileStore
{
    private ILogger Logger { get; } = logger;
    private IOptions<S3ClientOptions> Options { get; } = options;

    private IAmazonS3 GetClient()
    {
        var config = new AmazonS3Config
        {
            ServiceURL = Options.Value.ServiceUrl
        };
        var client = new AmazonS3Client(Options.Value.AccessKey, Options.Value.SecretKey, config);
        return client;
    }

    private async Task PutObject(string key, byte[] data)
    {
        using (Logger.Stopwatch(LogLevel.Debug, $"uploading {data.Length} to S3 key {key}"))
        {
            using var ms = new MemoryStream(data);
            var req = new PutObjectRequest
            {
                BucketName = options.Value.Bucket,
                Key = key,
                InputStream = ms,
                ContentType = "application/octet-stream"
            };
            await GetClient().PutObjectAsync(req);
        }
    }

    private async Task<bool> ObjectExists(string key)
    {
        using (Logger.Stopwatch(LogLevel.Debug, $"checking if S3 key {key} exists"))
        {
            try
            {
                var req = new GetObjectMetadataRequest()
                {
                    BucketName = options.Value.Bucket,
                    Key = key
                };
                await GetClient().GetObjectMetadataAsync(req);
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;
                throw;
            }
        }
    }
    
    public async Task<string> StoreChunk(byte[] chunk)
    {
        var hash = SHA256.HashData(chunk);
        var sb = new StringBuilder();
        foreach (var b in hash)
        {
            sb.Append(b.ToString("x2"));
        }
        var hashString = sb.ToString();

        if (await ObjectExists(hashString))
        {
            Logger.LogDebug("Object {hash} already exists in S3 bucket, not uploading", hashString);
            return hashString;
        }
        
        Logger.LogDebug("Uploading object {hash} to S3 bucket", hashString);
        await PutObject(hashString, chunk);
        return hashString;
    }
}