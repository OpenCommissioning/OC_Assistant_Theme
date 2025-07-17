using System.Windows;
using System.Windows.Media;

namespace OC.Assistant.Theme;

/// <summary>
/// Represents a <see cref="Grid"/> element to work as an overlay when the application is busy.
/// </summary>
public partial class BusyOverlay
{
    private readonly RotateTransform _rotateTransform = new ();
    private CancellationTokenSource? _cts;
    private static bool _hasBeenCreated;
    
    /// <summary>
    /// Creates a new instance of the <see cref="BusyOverlay"/>.
    /// </summary>
    public BusyOverlay()
    {
        if (_hasBeenCreated) throw new InvalidOperationException("BusyOverlay can only be created once");
        _hasBeenCreated = true;
        InitializeComponent();
        Size = 100;
        StateChanged += OnStateChanged;
    }

    /// <summary>
    /// Is triggered when the state of the <see cref="BusyOverlay"/> changes.
    /// </summary>
    public static event Action<bool>? StateChanged;

    /// <summary>
    /// Sets the state of the <see cref="BusyOverlay"/>.
    /// </summary>
    /// <param name="isBusy">
    /// <c>true</c> to activate the busy overlay; <c>false</c> to deactivate it.
    /// </param>
    public static void SetState(bool isBusy)
    {
        if (!_hasBeenCreated) throw new InvalidOperationException("BusyOverlay has not been created");
        StateChanged?.Invoke(isBusy);
    }
    
    private void OnStateChanged(bool isBusy)
    {
        Dispatcher.Invoke(() =>
        {
            if (!isBusy)
            {
                _cts?.Cancel();
                Visibility = Visibility.Hidden;
                return;
            }
            if (Visibility == Visibility.Visible) return;
            Visibility = Visibility.Visible;
            StartRotate();
        });
    }

    /// <summary>
    /// Sets the size of the animated icon. Default value is 100.
    /// </summary>
    public double Size
    {
        set
        {
            Grid.Height = value;
            Grid.Width = value;
            Grid.Margin = new Thickness(value, value, 0, 0);
            Label.Margin = new Thickness(-value, -value, 0, 0);
            Label.FontSize = value / 2;
        }
    }
    
    private void StartRotate()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        
        Task.Run(async () =>
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Dispatcher.Invoke(() =>
                    {
                        _rotateTransform.Angle += 3.0;
                        Grid.RenderTransform = _rotateTransform;
                    });

                    await Task.Delay(16, token);
                }
            }
            catch
            {
                // ignore
            }
        }, token);
    }
}