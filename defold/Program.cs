
using Defold.Services;
using Microsoft.EntityFrameworkCore;

namespace Defold;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.Services.Configure<S3HostedServiceOptions>(builder.Configuration.GetSection("S3Server"));
        builder.Services.Configure<DeduplicationOptions>(builder.Configuration.GetSection("Deduplication"));
        builder.Services.Configure<S3ClientOptions>(builder.Configuration.GetSection("S3Client"));
        
        builder.Services.AddSingleton<DefoldS3Server>();
        builder.Services.AddSingleton<IMetadataStore, PostgresMetadataStore>();
        builder.Services.AddSingleton<IFileStore, S3FileStore>();
        
        builder.Services.AddHostedService<S3HostedService>();
        builder.Services.AddDbContext<DefoldDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("postgres")));
        
        builder.Build().Run();
    }
}