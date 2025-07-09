using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace OC.Assistant.Theme;

/// <summary>
/// Represents a modal.
/// </summary>
public partial class Modal
{
    private readonly DoubleAnimation _fadeIn = new(0, 1, TimeSpan.FromMilliseconds(100));
    private readonly DoubleAnimation _fadeOut = new(1, 0, TimeSpan.FromMilliseconds(100));
    private static bool _hasBeenCreated;
    
    private static event Action<string, UIElement, MessageBoxButton, MessageBoxImage>? Shown;
    private static event Action<MessageBoxResult>? Closed;

    /// <summary>
    /// Shows the modal dialog with the specified parameters.
    /// </summary>
    /// <param name="caption">The title of the modal.</param>
    /// <param name="content">The content displayed within the modal, represented as a <see cref="UIElement"/>.</param>
    /// <param name="button">The <see cref="MessageBoxButton"/> options available in the modal.</param>
    /// <param name="image">The <see cref="MessageBoxImage"/> icon to display in the modal.</param>
    /// <returns>A <see cref="MessageBoxResult"/> indicating the user's interaction with the modal.</returns>
    /// <exception cref="InvalidOperationException">Is thrown when the modal has not been created in the current application.</exception>
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
    /// <param name="button">The <see cref="MessageBoxButton"/> options available in the modal.</param>
    /// <param name="image">The <see cref="MessageBoxImage"/> icon to display in the modal.</param>
    /// <returns>A <see cref="MessageBoxResult"/> indicating the user's interaction with the modal.</returns>
    /// <exception cref="InvalidOperationException">Is thrown when the modal has not been created in the current application.</exception>
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
        _fadeOut.Completed += FadeOutOnCompleted;
        Shown += OnShown;
    }

    private void OnShown(string caption, UIElement content, MessageBoxButton button, MessageBoxImage image)
    {
        TitleLabel.Text = caption;
        ContentGrid.Children.Add(content);
        SetButtons(button);
        SetImage(image);
        FadeIn();
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
        FadeOut();
    }
    
    private void FadeIn()
    {
        Visibility = Visibility.Visible;
        BeginAnimation(OpacityProperty, _fadeIn);
        if (BlurEffect is null) return;
        BlurEffect.Radius = 6;
    }
    
    private void FadeOut()
    {
        BeginAnimation(OpacityProperty, _fadeOut);
        if (BlurEffect is null) return;
        BlurEffect.Radius = 0;
    }
    
    private void FadeOutOnCompleted(object? sender, EventArgs e)
    {
        Visibility = Visibility.Collapsed;
        ContentGrid.Children.Clear();
    }
}