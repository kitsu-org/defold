using Microsoft.Extensions.Options;
using S3ServerLibrary;

namespace Defold;

public class S3HostedService : IHostedService
{
    private IOptionsMonitor<S3HostedServiceOptions> Options { get; set; }
    private ILogger<S3HostedService> Logger { get; }
    private S3Server S3Server { get; set; }

    public S3HostedService(
        IOptionsMonitor<S3HostedServiceOptions> options, 
        ILogger<S3HostedService> logger)
    {
        Options = options;
        Logger = logger;
        // options.OnChange(OnOptionsChanged);
    }

    // todo: set a flag here read from a loop in StartAsync that says we need to restart the server
    // private void OnOptionsChanged(S3HostedServiceOptions options)
    // {
    //     S3Server.Stop();
    //     S3Server = BuildS3Server(options);
    //     
    // }

    private void SetupServerCallbacks(S3Server server)
    {
        
    }
    
    private S3Server BuildS3Server(S3HostedServiceOptions options)
    {
        var settings = options.ToS3ServerSettings();
        settings.Logger = s => Logger.LogInformation("S3 library log message: {message}", s); 
        var server = new S3Server(settings);
        return server;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var settings = Options.CurrentValue.ToS3ServerSettings();
        S3Server = new S3Server(settings);
        S3Server.Webserver.Start(cancellationToken);
        return Task.CompletedTask;
    }

    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        S3Server.Webserver.Stop();

        return Task.CompletedTask;
    }
}