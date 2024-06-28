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

        #region Settings load and save
        // Load saved settings
        SquigglizerSettings? settings = SettingsSaver.Load<SquigglizerSettings>(SettingsSaver.LAST_SETTINGS_FILE);

        // Apply saved settings
        if (settings != null)
        {
            this.Width = settings.WindowWidth;
            this.Height = settings.WindowHeight;
            this.WindowState = settings.IsFullscreen ? WindowState.Maximized : WindowState.Normal;
            this.Left = settings.WindowX;
            this.Top = settings.WindowY;
            this.SolverSelector.SelectedItem = settings.Solver;
        }

        // Set up auto-save for settings
        void saveSettings()
        {
            SettingsSaver.Save(SettingsSaver.LAST_SETTINGS_FILE, new SquigglizerSettings(
                (int)this.Left,
                (int)this.Top,
                (int)this.Width,
                (int)this.Height,
                this.WindowState == WindowState.Maximized,
                this.SolverSelector.SelectedItem?.ToString()
            ));
        }

        this.SizeChanged += (_, _) => saveSettings();
        this.StateChanged += (_, _) => saveSettings();
        this.LocationChanged += (_, _) => saveSettings();
        this.SolverSelector.SelectionChanged += (_, _) => saveSettings();
        #endregion
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