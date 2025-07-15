using System.Windows;
using OC.Assistant.Theme;

namespace ThemeTest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        LogViewer.Add(this, "Application has started", MessageType.Info);
    }

    private async void ShowModalOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await Modal.Show("Modal", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            LogViewer.Add(this, ex.Message, MessageType.Error);
        }
    }

    private void ButtonMessageBoxOnClick(object sender, RoutedEventArgs e)
    {
        if (OC.Assistant.Theme.MessageBox
            .Show("MessageBox", "OK or Cancel?", MessageBoxButton.OKCancel, MessageBoxImage.Question)
            == MessageBoxResult.OK)
        {
            LogViewer.Add(this, "Clicked OK", MessageType.Info);
            return;
        }
        
        LogViewer.Add(this, "Clicked Cancel", MessageType.Warning);
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