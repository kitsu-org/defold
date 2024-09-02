using S3ServerLibrary;
using S3ServerLibrary.S3Objects;

namespace Defold.Callbacks;

public interface IServiceCallbacks
{
    /// <summary>
    /// List all buckets
    /// </summary>
    /// <param name="context"></param>
    Task<ListAllMyBucketsResult> ListBuckets(S3Context context);

    /// <summary>
    /// Service exists
    /// </summary>
    /// <param name="context"></param>
    Task<string> ServiceExists(S3Context context);

    /// <summary>
    /// Find matching base domain. The input string will be the hostname from the `host` header.
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    string FindMatchingBaseDomain(string host);

    /// <summary>
    /// Retrieve the base64-encoded secret key for a given requester.
    /// </summary>
    /// <param name="context"></param>
    string GetSecretKey(S3Context context);
}