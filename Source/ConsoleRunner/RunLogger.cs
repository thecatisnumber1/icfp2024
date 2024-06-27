using Lib;

namespace ConsoleRunner;

public class RunLogger : ILogger
{
    public void Break(bool immediate)
    {
        // Do nothing;
    }

    public void LogMessage(string logString)
    {
        Console.WriteLine(logString);
    }

    public void LogError(string logString)
    {
        Console.Error.WriteLine(logString);
    }
}
