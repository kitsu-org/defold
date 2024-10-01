using Defold.Models;
using Microsoft.EntityFrameworkCore;

namespace Defold;

public class DefoldDbContext(DbContextOptions<DefoldDbContext> options) : DbContext(options)
{
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<FileChunk> FileChunks { get; set; }
}