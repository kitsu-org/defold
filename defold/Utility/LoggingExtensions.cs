namespace Defold.Utility;

public static class LoggingExtensions
{
    public static StopwatchLogger Stopwatch(this ILogger logger, LogLevel level, string name)
    {
        return new StopwatchLogger(logger, level, name);
    }
}

