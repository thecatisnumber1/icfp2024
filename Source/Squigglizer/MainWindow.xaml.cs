using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Runner;

namespace Squigglizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Run? runner;

    public MainWindow()
    {
        InitializeComponent();

        string[] solverList = Run.SolverNames;
        SolverSelector.ItemsSource = solverList;
        SolverSelector.SelectedIndex = 0;
        ConsoleControl.LogMessage("Welcome to Squigglizer!");
    }

    private void SolverSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SolverRunButton.IsEnabled = true;
    }

    private async void SolverRunButton_OnClick(object sender, RoutedEventArgs e)
    {
        runner = new Run((string)SolverSelector.SelectedItem, [], ConsoleControl);

        RenderPane.Children.Clear();
        int multiplier = 3;

        Rectangle border = new()
        {
            Width = runner.Assignment.Width * multiplier,
            Height = runner.Assignment.Height * multiplier,
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };
        RenderPane.Children.Add(border);

        while (!runner.Assignment.IsSolved)
        {
            runner.Step();

            Rectangle rect = new()
            {
                Width = multiplier,
                Height = multiplier,
                Fill = runner.Assignment.IsSolved ? Brushes.Red : Brushes.Blue
            };
            Canvas.SetLeft(rect, runner.Assignment.PreviousGuess.X * multiplier);
            Canvas.SetBottom(rect, runner.Assignment.PreviousGuess.Y * multiplier);
            RenderPane.Children.Add(rect);

            await Task.Delay(500);
        }

        runner.RunUntilDone();
    }
}