using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using OC.Assistant.Theme.Internals;

namespace OC.Assistant.Theme;

/// <summary>
/// Represents the message type.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// The message shows an info.
    /// </summary>
    Info,
    /// <summary>
    /// The message shows a warning.
    /// </summary>
    Warning,
    /// <summary>
    /// The message shows an error.
    /// </summary>
    Error
}

/// <summary>
/// Represents a message.
/// </summary>
/// <param name="sender">The sender of the message.</param>
/// <param name="message">The message text.</param>
/// <param name="type">The message type.</param>
internal class Message(object sender, string message, MessageType type)
{
    private static readonly ControlTemplate? InfoIconTemplate = Application.Current.Resources["InformationIcon"] as ControlTemplate;
    private static readonly ControlTemplate? WarningIconTemplate = Application.Current.Resources["WarningIcon"] as ControlTemplate;
    private static readonly ControlTemplate? ErrorIconTemplate = Application.Current.Resources["ErrorIcon"] as ControlTemplate;
    
    /// <summary>
    /// Gets a <see cref="DateTime"/> value that represents the time of creation of this instance.
    /// </summary>
    private DateTime DateTime { get; } = DateTime.Now;

    /// <summary>
    /// Gets the type of this message.
    /// </summary>
    public MessageType Type => type;
    
    /// <summary>
    /// Gets the header of the message containing timestamp and sender information.
    /// </summary>
    public string Header => $"{DateTime:yyyy-MM-dd HH:mm:ss.fff} | {sender.ToString()?.Split(':')[0]}";
    
    /// <summary>
    /// Gets the message text.
    /// </summary>
    public string Text => message;

    /// <summary>
    /// Gets the icon template for the message type.
    /// </summary>
    public ControlTemplate? IconTemplate => Type switch
    {
        MessageType.Info => InfoIconTemplate,
        MessageType.Warning => WarningIconTemplate,
        MessageType.Error => ErrorIconTemplate,
        _ => null
    };
}

/// <summary>
/// Represents a <see cref="UserControl"/> to display log messages.
/// </summary>
public partial class LogViewer
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Queue<Message> _messageQueue = [];
    private readonly object _queueLock = new ();
    private readonly ObservableRangeExtension<Message> _messageBuffer =[];
    private readonly ICollectionView _collectionView;
    private bool _infoFilterEnabled = true;
    private bool _warningFilterEnabled = true;
    private bool _errorFilterEnabled = true;
    private ScrollViewer? _scrollViewer;

    /// <summary>
    /// Creates a new instance of the <see cref="LogViewer"/>.
    /// </summary>
    public LogViewer()
    {
        InitializeComponent();
        
        ItemsControl.ItemsSource = _messageBuffer;
        _messageBuffer.CollectionChanged += (_, _) => _scrollViewer?.ScrollToBottom();
        _collectionView = CollectionViewSource.GetDefaultView(ItemsControl.ItemsSource);
        _collectionView.Filter = UserFilter;
    }

    /// <summary>
    /// Adds a new message to the <see cref="LogViewer"/>.
    /// </summary>
    /// <param name="sender">The sender of the message.</param>
    /// <param name="message">The message text.</param>
    /// <param name="type">The message type.</param>
    public void Add(object sender, string message, MessageType type)
    {
        lock (_queueLock)
        {
            _messageQueue.Enqueue(new Message(sender, message, type));
        }
    }

    private void Console_OnLoaded(object sender, RoutedEventArgs e)
    {
        _scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(ItemsControl, 0);
        Task.Run(UpdateLoop);
    }

    private bool UserFilter(object item)
    {
        var type = ((Message)item).Type;
        return (type is MessageType.Info && _infoFilterEnabled) ||
               (type is MessageType.Warning && _warningFilterEnabled) ||
               (type is MessageType.Error && _errorFilterEnabled);
    }
    
    private async Task UpdateLoop()
    {
        var token = _cancellationTokenSource.Token;

        while (!token.IsCancellationRequested)
        {
            try { await Task.Delay(100, token); }
            catch (TaskCanceledException) { }

            Message[] messages;
            
            lock (_queueLock)
            {
                if (_messageQueue.Count == 0) continue;
                messages = _messageQueue.ToArray();
                _messageQueue.Clear();
            }
            
            Dispatcher.Invoke(() =>
            {
                _messageBuffer.AddRange(messages);
                if (_messageBuffer.Count > 10000)
                {
                    _messageBuffer.RemoveRange(0, _messageBuffer.Count - 8000);
                }
            });
        }
    }

    private async void ClearOnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (await MessageBox.Show("LogViewer", "Clear all messages?", 
                    MessageBoxButton.OKCancel, 
                    MessageBoxImage.Question) 
                != MessageBoxResult.OK) return;
            _messageBuffer.Clear();
        }
        catch (Exception ex)
        {
            Add(this, ex.Message, MessageType.Error);
        }
    }
    
    private void ErrorFilterOnClick(object sender, RoutedEventArgs e)
    {
        _errorFilterEnabled = ((ToggleButton) sender).IsChecked == true;
        _collectionView.Refresh();
    }

    private void WarningFilterOnClick(object sender, RoutedEventArgs e)
    {
        _warningFilterEnabled = ((ToggleButton) sender).IsChecked == true;
        _collectionView.Refresh();
    }
        
    private void InfoFilterOnClick(object sender, RoutedEventArgs e)
    {
        _infoFilterEnabled = ((ToggleButton) sender).IsChecked == true;
        _collectionView.Refresh();
    }
}