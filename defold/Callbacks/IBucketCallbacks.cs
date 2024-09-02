using S3ServerLibrary;
using S3ServerLibrary.S3Objects;

namespace Defold.Callbacks;

public interface IBucketCallbacks
{
    /// <summary>
    /// Delete a bucket
    /// </summary>
    /// <param name="context"></param>
    Task DeleteAsync(S3Context context);
    
    /// <summary>
    /// Delete a bucket's ACL
    /// </summary>
    /// <param name="context"></param>
    Task DeleteAclAsync(S3Context context);
    
    /// <summary>
    /// Delete a bucket's tags
    /// </summary>
    /// <param name="context"></param>
    Task DeleteTagging(S3Context context);
    
    /// <summary>
    /// Delete a bucket's website configuration
    /// </summary>
    /// <param name="context"></param>
    Task DeleteWebsite(S3Context context);
    
    /// <summary>
    /// Check for the existence of a bucket.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>True if the bucket exists, false if not.</returns>
    Task<bool> Exists(S3Context context);
    
    /// <summary>
    /// Enumerate a bucket
    /// </summary>
    /// <param name="context"></param>
    /// <returns>List of files in a bucket</returns>
    Task<ListBucketResult> Read(S3Context context);
    
    /// <summary>
    /// Read a bucket's access control policy
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Bucket's ACL</returns>
    Task<AccessControlPolicy> ReadAcl(S3Context context);
    
    /// <summary>
    /// Retrieve logging configuration for a bucket
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Bucket's logging configuration</returns>
    Task<BucketLoggingStatus> ReadLogging(S3Context context);
    
    /// <summary>
    /// Retrieve location (region) constraint from the server for this bucket
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Location information for the bucket</returns>
    Task<LocationConstraint> ReadLocation(S3Context context);
    
    /// <summary>
    /// Retrieve multipart uploads
    /// </summary>
    /// <param name="context"></param>
    /// <returns>List of multipart uploads</returns>
    Task<ListMultipartUploadsResult> ReadMultipartUploads(S3Context context);
    
    /// <summary>
    /// Read a bucket's tags
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<Tagging> ReadTagging(S3Context context);
    
    /// <summary>
    /// Get a list of object versions in the bucket
    /// </summary>
    /// <param name="context"></param>
    /// <returns>A list of object versions</returns>
    Task<ListVersionsResult> ReadVersions(S3Context context);
    
    /// <summary>
    /// Get a bucket's versioning policy.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Bucket versioning policy</returns>
    Task<VersioningConfiguration> ReadVersioning(S3Context context);
    
    /// <summary>
    /// Get a bucket's website configuration
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Bucket website configuration</returns>
    Task<WebsiteConfiguration> ReadWebsite(S3Context context);
    
    /// <summary>
    /// Write a bucket
    /// </summary>
    /// <param name="context"></param>
    Task Write(S3Context context);
    
    /// <summary>
    /// Write an ACL to a bucket, deleting the previous ACL
    /// </summary>
    /// <param name="context"></param>
    /// <param name="policy"></param>
    Task WriteAcl(S3Context context, AccessControlPolicy policy);
    
    /// <summary>
    /// Write logging configuration to a bucket, deleting the previous configuration.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logging"></param>
    Task WriteLogging(S3Context context, BucketLoggingStatus logging);
    
    /// <summary>
    /// Write tags to a bucket, deleting the previous tags.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="tagging"></param>
    Task WriteTagging(S3Context context, Tagging tagging);
    
    /// <summary>
    /// Set a bucket's versioning policy
    /// </summary>
    /// <param name="context"></param>
    /// <param name="versioning"></param>
    Task WriteVersioning(S3Context context, VersioningConfiguration versioning);
    
    /// <summary>
    /// Set a bucket's website configuration.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    Task WriteWebsite(S3Context context, WebsiteConfiguration configuration);
}