namespace Defold.Services;

public interface IMetadataStore
{
    Task AddFile(Models.UploadedFile file);
}