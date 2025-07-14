using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace OC.Assistant.Theme;

/// <summary>
/// Represents a modal.
/// </summary>
public partial class Modal
{
    private static bool _hasBeenCreated;
    
    private static event Action<string, UIElement, MessageBoxButton, MessageBoxImage>? Shown;
    private static event Action<MessageBoxResult>? Closed;

    /// <summary>
    /// Shows the modal dialog with the specified parameters.
    /// </summary>
    /// <param name="caption">The title of the modal.</param>
    /// <param name="content">The content displayed within the modal, represented as a <see cref="UIElement"/>.</param>
    /// <param name="button">The <see cref="MessageBoxButton"/> options.<br/>
    /// Only accepts <see cref="MessageBoxButton.OK"/> and <see cref="MessageBoxButton.OKCancel"/>.</param>
    /// <param name="image">The <see cref="MessageBoxImage"/> icon to display.<br/>
    /// Only accepts <see cref="MessageBoxImage.Information"/>, <see cref="MessageBoxImage.Warning"/>,
    /// <see cref="MessageBoxImage.Error"/> and <see cref="MessageBoxImage.Question"/>.</param>
    /// <returns>A <see cref="MessageBoxResult"/> indicating the user's interaction with the modal.</returns>
    /// <exception cref="InvalidOperationException">Is thrown when the modal has not been created
    /// or the modal has not been closed.</exception>
    public static async Task<MessageBoxResult> Show(string caption, UIElement content, MessageBoxButton button, MessageBoxImage image)
    {
        if (!_hasBeenCreated) throw new InvalidOperationException("Modal has not been created");
        Shown?.Invoke(caption, content, button, image);
        return await WaitForClosed();
    }
    
    /// <summary>
    /// Shows the modal dialog with the specified parameters.
    /// </summary>
    /// <param name="caption">The title of the modal.</param>
    /// <param name="text">The text displayed within the modal.</param>
    /// <param name="button">The <see cref="MessageBoxButton"/> options.<br/>
    /// Only accepts <see cref="MessageBoxButton.OK"/> and <see cref="MessageBoxButton.OKCancel"/>.</param>
    /// <param name="image">The <see cref="MessageBoxImage"/> icon to display.<br/>
    /// Only accepts <see cref="MessageBoxImage.Information"/>, <see cref="MessageBoxImage.Warning"/>,
    /// <see cref="MessageBoxImage.Error"/> and <see cref="MessageBoxImage.Question"/>.</param>
    /// <returns>A <see cref="MessageBoxResult"/> indicating the user's interaction with the modal.</returns>
    /// <exception cref="InvalidOperationException">Is thrown when the modal has not been created
    /// or the modal has not been closed.</exception>
    public static async Task<MessageBoxResult> Show(string caption, string text, MessageBoxButton button, MessageBoxImage image)
    {
        var content = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center, 
            TextWrapping = TextWrapping.Wrap,
            Text = text
        };
        return await Show(caption, content, button, image);
    }
    
    /// <summary>
    /// Gets or sets the <see cref="BlurEffect"/> to be affected when the modal is shown.
    /// </summary>
    public BlurEffect? BlurEffect { get; set; }
    
    /// <summary>
    /// Creates a new instance of the <see cref="Modal"/>.
    /// </summary>
    public Modal()
    {
        if (_hasBeenCreated)
        {
            throw new InvalidOperationException("Modal can only be created once");
        }
        _hasBeenCreated = true;
        InitializeComponent();
        Visibility = Visibility.Collapsed;
        Shown += OnShown;
    }

    private void OnShown(string caption, UIElement content, MessageBoxButton button, MessageBoxImage image)
    {
        if (ContentGrid.Children.Count > 0)
        {
            throw new InvalidOperationException("Modal has not been closed");
        }
        TitleLabel.Text = caption;
        ContentGrid.Children.Add(content);
        SetButtons(button);
        SetImage(image);
        Show();
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

    private void SetButtons(MessageBoxButton button)
    {
        ButtonOk.Visibility = Visibility.Visible;
        ButtonCancel.Visibility = button switch
        {
            MessageBoxButton.OKCancel => Visibility.Visible,
            _ => Visibility.Collapsed
        };
    }

    private void SetImage(MessageBoxImage image)
    {
        SymbolInformation.Visibility = image == MessageBoxImage.Information ? Visibility.Visible : Visibility.Collapsed;
        SymbolWarning.Visibility = image == MessageBoxImage.Warning ? Visibility.Visible : Visibility.Collapsed;
        SymbolError.Visibility = image == MessageBoxImage.Error ? Visibility.Visible : Visibility.Collapsed;
        SymbolQuestion.Visibility = image == MessageBoxImage.Question ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ButtonOnClick(object sender, RoutedEventArgs e)
    {
        var result = sender.Equals(ButtonOk) ? MessageBoxResult.OK : MessageBoxResult.Cancel;
        Closed?.Invoke(result);
        Hide();
    }
    
    private void Show()
    {
        Visibility = Visibility.Visible;
        if (BlurEffect is null) return;
        BlurEffect.Radius = 6;
    }
    
    private void Hide()
    {
        Visibility = Visibility.Collapsed;
        ContentGrid.Children.Clear();
        if (BlurEffect is null) return;
        BlurEffect.Radius = 0;
    }
}