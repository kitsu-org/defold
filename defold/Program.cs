
namespace Defold;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.Configure<S3HostedServiceOptions>(builder.Configuration.GetSection("S3Server"));
        
        builder.Services.AddHostedService<S3HostedService>();
        builder.Build().Run();
    }
}