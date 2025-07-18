using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OC.Assistant.Theme;

/// <summary>
/// Represents a custom message box with an appropriate theme style.
/// </summary>
public partial class MessageBox
{
    private MessageBox()
    {
        InitializeComponent();
        this.SetDarkAttribute();
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
    /// Indicates whether the message box is currently open or not.
    /// </summary>
    /// <returns>True if the message box is open, otherwise false.</returns>
    public static bool IsOpen { get; private set; }
    
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
        var messageBox = new MessageBox
        {
            TitleLabel = { Text = caption },
            Title = caption,
        };
        messageBox.ContentGrid.Children.Add(content);
        SetButtons(messageBox, button);
        SetImage(messageBox, image);
        return await SetPositionAndShow(messageBox);
    }
    
    private static async Task<MessageBoxResult> SetPositionAndShow(MessageBox messageBox)
    {
        var mainWindow = Application.Current.MainWindow;
        if (mainWindow is null || messageBox.Content is not FrameworkElement content) {return MessageBoxResult.None;}
        messageBox.Owner = mainWindow;
        
        content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        messageBox.WindowStartupLocation = WindowStartupLocation.Manual;
        messageBox.Top = mainWindow.Top + mainWindow.Height / 2 - content.DesiredSize.Height / 2;
        messageBox.Left = mainWindow.Left + mainWindow.Width / 2 - content.DesiredSize.Width / 2;
        messageBox.Height = content.DesiredSize.Height + 2;
        messageBox.Width = content.DesiredSize.Width + 2;
        IsOpen = true;
        messageBox.Show();
        Shown?.Invoke();
        return await WaitForClosed();
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
        switch (image)
        {
            case MessageBoxImage.Question:
                messageBox.IconControl.Template = Application.Current.Resources["QuestionIcon"] as ControlTemplate;
                break;
            case MessageBoxImage.Information:
                messageBox.IconControl.Template = Application.Current.Resources["InformationIcon"] as ControlTemplate;
                break;
            case MessageBoxImage.Warning:
                messageBox.IconControl.Template = Application.Current.Resources["WarningIcon"] as ControlTemplate;
                break;
            case MessageBoxImage.Error:
                messageBox.IconControl.Template = Application.Current.Resources["ErrorIcon"] as ControlTemplate;
                break;
            case MessageBoxImage.None:
            default:
                messageBox.IconControl.Visibility = Visibility.Collapsed;
                break;
        }
    }

    private void ButtonOnClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBoxResult.None;
        if (sender.Equals(ButtonOk)) result = MessageBoxResult.OK;
        else if (sender.Equals(ButtonYes)) result = MessageBoxResult.Yes;
        else if (sender.Equals(ButtonNo)) result = MessageBoxResult.No;
        else if (sender.Equals(ButtonCancel)) result = MessageBoxResult.Cancel;
        
        IsOpen = false;
        Closed?.Invoke(result);
        Close();
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