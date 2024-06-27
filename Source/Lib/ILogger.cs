namespace Lib;

public interface ILogger
{
    /// <summary>
    /// Pauses immediately until the logger returns
    /// </summary>
    /// <param name="immediate">If true, blocks in the Logger. Else blocks opportunistically.</param>
    /// <remarks>If false, the visualizer will only block if the user has selected the Pause button.</remarks>
    void Break(bool immediate = false);

    void LogMessage(string logString);

    void LogError(string logString);
}