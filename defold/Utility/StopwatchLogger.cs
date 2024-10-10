using System.Diagnostics;

namespace Defold.Utility;

public class StopwatchLogger: IDisposable
{
    private ILogger Logger { get; }
    private LogLevel Level { get; }
    private string Name { get; }
    private Stopwatch Stopwatch { get; }

    public StopwatchLogger(ILogger logger, LogLevel level, string name)
    {
        Logger = logger;
        Level = level;
        Name = name;
        
        Logger.StartStopwatch(Level, Name);
        Stopwatch = Stopwatch.StartNew();
    }
    
    public void Dispose()
    {
        Stopwatch.Stop();
        Logger.StopStopwatch(Level, Name, Stopwatch.ElapsedMilliseconds);
    }
}

public static partial class StopwatchLoggerMessages
{
    [LoggerMessage(EventId = 0, Message = "Starting stopwatch logger {name}")]
    public static partial void StartStopwatch(this ILogger logger, LogLevel level, string name);

    [LoggerMessage(EventId = 1, Message = "Stopped stopwatch logger {name} - elapsed time {time}ms")]
    public static partial void StopStopwatch(this ILogger logger, LogLevel level, string name, long time);
}