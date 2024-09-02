using S3ServerLibrary;
using WatsonWebserver.Core;

namespace Defold;

public class S3HostedServiceOptions
{
    public S3HostedServiceLoggingOptions? Logging { get; set; }
    public S3HostedServiceLimitsOptions? Limits { get; set; }
    public S3HostedServiceWebServerSettings? Server { get; set; }
    public bool? EnableSignatures { get; set; }

    public S3ServerSettings ToS3ServerSettings()
    {
        var settings = new S3ServerSettings();
        if (Logging != null)
            settings.Logging = Logging.GetLoggingSettings();
        if (Limits != null)
            settings.OperationLimits = Limits.GetOperationLimitsSettings();
        if (Server != null)
            settings.Webserver = Server.GetWebserverSettings();
        if (EnableSignatures != null)
            settings.EnableSignatures = EnableSignatures.Value;

        return settings;
    }
}

public class S3HostedServiceLoggingOptions
{
    public bool? EnableHttpRequestLogging { get; set; }
    public bool? EnableS3RequestLogging { get; set; }
    public bool? EnableV4ValidationLogging { get; set; }
    public bool? EnableExceptionLogging { get; set; }

    public LoggingSettings GetLoggingSettings()
    {
        var settings = new LoggingSettings();
        if (EnableHttpRequestLogging != null)
            settings.HttpRequests = EnableHttpRequestLogging.Value;
        if (EnableS3RequestLogging != null)
            settings.S3Requests = EnableS3RequestLogging.Value;
        if (EnableV4ValidationLogging != null)
            settings.SignatureV4Validation = EnableV4ValidationLogging.Value;
        if (EnableExceptionLogging != null)
            settings.Exceptions = EnableExceptionLogging.Value;

        return settings;
    }
}

public class S3HostedServiceLimitsOptions
{
    public long? MaxPutObjectSize { get; set; }

    public OperationLimitsSettings GetOperationLimitsSettings()
    {
        var settings = new OperationLimitsSettings();
        
        if (MaxPutObjectSize != null)
            settings.MaxPutObjectSize = MaxPutObjectSize.Value;
        
        return settings;
    }
}

public class S3HostedServiceWebServerSettings
{
    public string? Hostname { get; set; }
    public int? Port { get; set; }

    public WebserverSettings GetWebserverSettings()
    {
        var settings = new WebserverSettings();
        if (Hostname != null)
            settings.Hostname = Hostname;
        if (Port != null)
            settings.Port = Port.Value;
        return settings;
    }
}