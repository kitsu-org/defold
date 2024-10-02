using Defold.Models;
using Microsoft.EntityFrameworkCore;

namespace Defold.Services;

public class PostgresMetadataStore(IServiceProvider serviceProvider) : IMetadataStore
{
    // this is a workaround for the fact that we can't pull in the scoped DbContext
    // from this singleton service, but we're stuck with a singleton due to how 
    // the S3Server library is implemented. this is just a temporary workaround
    // for now.
    private IServiceProvider ServiceProvider { get; } = serviceProvider;
    
    public async Task AddFile(UploadedFile file)
    {
        using var scope = ServiceProvider.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<DefoldDbContext>();
        
        // delete uploaded files/file chunks before inserting to prevent overwriting a file
        // from causing an error.
        // because deletes cascade, this also clears any FileChunk references for it.
        await context.UploadedFiles
            .Where(uf => uf.Bucket == file.Bucket && uf.Key == file.Key)
            .ExecuteDeleteAsync();
        
        await context.UploadedFiles.AddAsync(file);
        await context.SaveChangesAsync();
    }
}