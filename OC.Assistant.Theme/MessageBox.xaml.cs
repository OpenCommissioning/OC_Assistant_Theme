using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OC.Assistant.Theme;


/// <summary>
/// Represents a custom message box with an appropriate theme style.
/// </summary>
public partial class MessageBox
{
    private static MessageBox? _messageBox;
    
    private MessageBox()
    {
        InitializeComponent();
    }

    /// <summary>
    /// A static event that is triggered when the message box is displayed to the user.
    /// It allows handlers to respond right before or immediately as the message box becomes visible.
    /// </summary>
    public static event Action? Shown;
    
    /// <summary>
    /// A static event that occurs when the message box is closed.
    /// The event provides a <see cref="MessageBoxResult"/> indicating the user's action or decision.
    /// </summary>
    public new static event Action<MessageBoxResult>? Closed;
    
    /// <summary>
    /// Shows the message box with the given parameters. 
    /// </summary>
    /// <param name="caption">The caption of the message box.</param>
    /// <param name="text">The text within the message box.</param>
    /// <param name="button">The <see cref="MessageBoxButton"/> of the message box.</param>
    /// <param name="image">The shown image on the left side.</param>
    /// <returns></returns>
    public static async Task<MessageBoxResult> Show(string caption, string text, MessageBoxButton button, MessageBoxImage image)
    {
        return await Show(caption, new Label { VerticalAlignment = VerticalAlignment.Center, Content = text}, button, image);
    }
    
    /// <summary>
    /// Shows the message box with the given parameters. 
    /// </summary>
    /// <param name="caption">The caption of the message box.</param>
    /// <param name="content">The content within the message box.</param>
    /// <param name="button">The <see cref="MessageBoxButton"/> of the message box.</param>
    /// <param name="image">The shown image on the left side.</param>
    /// <returns></returns>
    public static async Task<MessageBoxResult> Show(string caption, UIElement content, MessageBoxButton button, MessageBoxImage image)
    {
        _messageBox = new MessageBox
        {
            TitleLabel = { Text = caption },
            Title = caption
        };
        _messageBox.ContentGrid.Children.Add(content);
        SetButtons(_messageBox, button);
        SetImage(_messageBox, image);
        return await SetPositionAndShow();
    }
    
    private static async Task<MessageBoxResult> SetPositionAndShow()
    {
        var result = MessageBoxResult.None;
        var mainWindow = Application.Current.MainWindow;
        if (mainWindow is null || _messageBox?.Content is not FrameworkElement content) return result;

        mainWindow.Activated += MainWindowOnActivated;
        
        content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        _messageBox.WindowStartupLocation = WindowStartupLocation.Manual;
        _messageBox.Top = mainWindow.Top + mainWindow.Height / 2 - content.DesiredSize.Height / 2;
        _messageBox.Left = mainWindow.Left + mainWindow.Width / 2 - content.DesiredSize.Width / 2;
        _messageBox.Height = content.DesiredSize.Height + 2;
        _messageBox.Width = content.DesiredSize.Width + 2;
        _messageBox.Show();
        
        Shown?.Invoke();
        
        result = await WaitForClosed();
        mainWindow.Activated -= MainWindowOnActivated;
        return result;
    }
    
    private static async Task<MessageBoxResult> WaitForClosed()
    {
        var tcs = new TaskCompletionSource<MessageBoxResult>();
        Closed += OnClosed;
        return await tcs.Task;

        void OnClosed(MessageBoxResult result)
        {
            Closed -= OnClosed;
            tcs.SetResult(result);
        }
    }

    private static void MainWindowOnActivated(object? sender, EventArgs e)
    {
        _messageBox?.Activate();
    }

    private static void SetButtons(MessageBox messageBox, MessageBoxButton button)
    {
        switch (button)
        {
            case MessageBoxButton.OK:
                messageBox.ButtonCancel.Visibility = Visibility.Collapsed;
                messageBox.ButtonNo.Visibility = Visibility.Collapsed;
                messageBox.ButtonYes.Visibility = Visibility.Collapsed;
                break;
            case MessageBoxButton.OKCancel:
                messageBox.ButtonNo.Visibility = Visibility.Collapsed;
                messageBox.ButtonYes.Visibility = Visibility.Collapsed;
                break;
            case MessageBoxButton.YesNo:
                messageBox.ButtonOk.Visibility = Visibility.Collapsed;
                messageBox.ButtonCancel.Visibility = Visibility.Collapsed;
                break;
            case MessageBoxButton.YesNoCancel:
                messageBox.ButtonOk.Visibility = Visibility.Collapsed;
                break;
            default:
                messageBox.ButtonCancel.Visibility = Visibility.Collapsed;
                messageBox.ButtonNo.Visibility = Visibility.Collapsed;
                messageBox.ButtonYes.Visibility = Visibility.Collapsed;
                break;
        }
    }

    private static void SetImage(MessageBox messageBox, MessageBoxImage image)
    {
        messageBox.SymbolInformation.Visibility =
            image == MessageBoxImage.Information ? Visibility.Visible : Visibility.Collapsed;
        
        messageBox.SymbolWarning.Visibility =
            image == MessageBoxImage.Warning ? Visibility.Visible : Visibility.Collapsed;
        
        messageBox.SymbolError.Visibility =
            image == MessageBoxImage.Error ? Visibility.Visible : Visibility.Collapsed;
        
        messageBox.SymbolQuestion.Visibility =
            image == MessageBoxImage.Question ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ButtonOnClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBoxResult.None;
        if (sender.Equals(ButtonOk)) result = MessageBoxResult.OK;
        else if (sender.Equals(ButtonYes)) result = MessageBoxResult.Yes;
        else if (sender.Equals(ButtonNo)) result = MessageBoxResult.No;
        else if (sender.Equals(ButtonCancel)) result = MessageBoxResult.Cancel;
        
        Closed?.Invoke(result);
        _messageBox?.Close();
        _messageBox = null;
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void CloseButtonOnClick(object sender, RoutedEventArgs e)
    {
        ButtonOnClick(sender, e);
    }
}