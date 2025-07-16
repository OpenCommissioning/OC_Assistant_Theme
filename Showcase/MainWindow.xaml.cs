using System.Windows;
using System.Windows.Controls;
using OC.Assistant.Theme;

namespace Showcase;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        LogViewer.Add(this, "Application has started", MessageType.Info);
        LogViewer.Add(this, "Application has started", MessageType.Warning);
        LogViewer.Add(this, "Application has started", MessageType.Error);
    }

    private async void ShowSettingsOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var settings = new StackPanel
            {
                Children =
                {
                    new TextBox
                    {
                        Margin = new Thickness(0, 10, 0, 0),
                        Text = "Value 1",
                    },
                    new TextBox
                    {
                        Margin = new Thickness(0, 10, 0, 0),
                        Text = "Value 2",
                    }
                }
            };
            await OC.Assistant.Theme.MessageBox
                .Show("Settings", settings, MessageBoxButton.OKCancel, MessageBoxImage.None);
        }
        catch (Exception ex)
        {
            LogViewer.Add(this, ex.Message, MessageType.Error);
        }
    }

    private async void ButtonMessageBoxOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (await OC.Assistant.Theme.MessageBox
                    .Show("MessageBox", "OK to continue", MessageBoxButton.OKCancel, MessageBoxImage.Information)
                != MessageBoxResult.OK) return;
        
            if (await OC.Assistant.Theme.MessageBox
                    .Show("MessageBox", "Yes to continue", MessageBoxButton.YesNoCancel, MessageBoxImage.Question)
                != MessageBoxResult.Yes) return;
        
            if (await OC.Assistant.Theme.MessageBox
                    .Show("MessageBox", "Yes to continue", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;
        
            await OC.Assistant.Theme.MessageBox
                .Show("MessageBox", "OK", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            LogViewer.Add(this, ex.Message, MessageType.Error);
        }
    }

    private async void ButtonBusyOverlayOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            BusyOverlay.SetState(true);
            LogViewer.Add(this, "Start async process...", MessageType.Info);
            await Task.Delay(1000);
            LogViewer.Add(this, "Async process has finished", MessageType.Info);
            BusyOverlay.SetState(false);
        }
        catch (Exception ex)
        {
            LogViewer.Add(this, ex.Message, MessageType.Error);
        }
    }

    private void LinkButtonOnClick(object sender, RoutedEventArgs e)
    {
        const string url = "https://github.com/OpenCommissioning/OC_Assistant_Theme";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {FileName = url, UseShellExecute = true});
    }
}