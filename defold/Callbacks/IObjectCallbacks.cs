using S3ServerLibrary;
using S3ServerLibrary.S3Objects;

namespace Defold.Callbacks;

public interface IObjectCallbacks
{
    Task AbortMultipartUpload(S3Context context);
    Task<CompleteMultipartUploadResult> CompleteMultipartUpload(S3Context context, CompleteMultipartUpload upload);
    
}