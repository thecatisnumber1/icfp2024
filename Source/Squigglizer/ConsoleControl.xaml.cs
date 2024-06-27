using Lib;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Squigglizer;

/// <summary>
/// Interaction logic for ConsoleControl.xaml
/// </summary>
public partial class ConsoleControl : UserControl, ILogger
{
    public ConsoleControl()
    {
        InitializeComponent();
    }

    public void Break(bool immediate)
    {
        // Do nothing;
    }

    public void LogMessage(string logString)
    {
        WriteFormattedLine(logString, Brushes.Black);
    }

    public void LogError(string logString)
    {
        WriteFormattedLine(logString, Brushes.Red);
    }

    private void WriteFormattedLine(string text, Brush color)
    {
        WriteFormattedText(text + "\n", color);
    }

    private void WriteFormattedText(string text, Brush color)
    {
        Paragraph.Inlines.Add(new Run(text) { Foreground = color });
    }
}
