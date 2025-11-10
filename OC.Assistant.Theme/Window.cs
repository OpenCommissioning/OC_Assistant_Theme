using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shell;
using OC.Assistant.Theme.Internals;

namespace OC.Assistant.Theme;

/// <inheritdoc />
/// <remarks>
/// Implements a custom TitleBar and a <see cref="BusyOverlay"/>.<br/><br/>
/// Content inside the TitleBar can be rendered by setting the <see cref="TitleBarContent"/>.<br/><br/>
/// To activate/deactivate the <see cref="BusyOverlay"/>, call
/// <see cref="BusyOverlay.SetState(bool)"/> anywhere in your application. 
/// </remarks>
public abstract class Window : System.Windows.Window
{
    /// <summary>
    /// Gets or sets a value indicating whether the application's icon is displayed in the title bar.
    /// </summary>
    public bool ShowIcon { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the application's title is displayed in the title bar.
    /// </summary>
    public bool ShowTitle { get; set; }
    
    /// <summary>
    /// Identifies the <see cref="TitleBarContent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleBarContentProperty =
        DependencyProperty.Register(
            nameof(TitleBarContent),
            typeof(object),
            typeof(Window),
            new PropertyMetadata(null));
    
    /// <summary>
    /// Gets or sets the custom content to be displayed in the title bar of the window.
    /// </summary>
    public object TitleBarContent
    {
        get => GetValue(TitleBarContentProperty);
        set => SetValue(TitleBarContentProperty, value);
    }
    
    /// <inheritdoc />
    protected Window()
    {
        InitializeComponent();
        this.SetDarkAttribute();
    }
    
    private void InitializeComponent()
    {
        SetResourceReference(StyleProperty, "DefaultWindowStyle");
        
        var titleBarHeight = (double)FindResource("TitleBarHeight");
        var foreground = (SolidColorBrush)FindResource("ForegroundBaseBrush");
        var white3 = (SolidColorBrush)FindResource("White3Brush");
        var white5 = (SolidColorBrush)FindResource("White5Brush");
        var white6 = (SolidColorBrush)FindResource("White6Brush");
        
        WindowChrome.SetWindowChrome(this, new WindowChrome
        {
            CaptionHeight = titleBarHeight,
            ResizeBorderThickness = new Thickness(5),
            GlassFrameThickness = new Thickness(1)
        });

        var rootGrid = new Grid();

        var blockingElement = new Grid
        {
            Focusable = true,
            Background = Brushes.Transparent,
            Visibility = Visibility.Hidden
        };
        WindowChrome.SetIsHitTestVisibleInChrome(blockingElement, true);
        
        var contentGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(titleBarHeight) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            }
        };

        var titleBarGrid = new Grid
        {
            Background = white6,
        };
        Grid.SetRow(titleBarGrid, 0);
        
        var customContentGrid = new Grid();
        Grid.SetRow(customContentGrid, 1);
        
        var titleBarCustomGrid = new Grid
        {
            Background = null,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 140, 0),
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            }
        };
        
        var appIcon = new Image
        {
            Visibility = Visibility.Collapsed,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(8, 6, 8, 6)
        };
        Grid.SetColumn(appIcon, 0);

        var title = new Label
        {
            Visibility = Visibility.Collapsed,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(title, 1);
        
        var titleBarContent = new ContentPresenter
        {
            Content = TitleBarContent
        };
        titleBarContent.SetBinding(ContentPresenter.ContentProperty, new Binding(nameof(TitleBarContent)) { Source = this });
        WindowChrome.SetIsHitTestVisibleInChrome(titleBarContent, true);
        Grid.SetColumn(titleBarContent, 2);
        
        var buttonStack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        Grid.SetColumn(buttonStack, 2);

        var minimizeButton = new Button
        {
            Style = (Style)FindResource("MinimizeButtonStyle")
        };
        minimizeButton.Click += MinimizeOnClick;
        WindowChrome.SetIsHitTestVisibleInChrome(minimizeButton, true);

        var restoreButton = new Button
        {
            Style = (Style)FindResource("RestoreButtonStyle"),
            Visibility = Visibility.Collapsed
        };
        restoreButton.Click += RestoreOnClick;
        WindowChrome.SetIsHitTestVisibleInChrome(restoreButton, true);

        var maximizeButton = new Button
        {
            Style = (Style)FindResource("MaximizeButtonStyle"),
            Visibility = Visibility.Visible
        };
        maximizeButton.Click += MaximizeOnClick;
        WindowChrome.SetIsHitTestVisibleInChrome(maximizeButton, true);

        var closeButton = new Button
        {
            Style = (Style)FindResource("CloseButtonStyle")
        };
        closeButton.Click += CloseOnClick;
        WindowChrome.SetIsHitTestVisibleInChrome(closeButton, true);
        
        buttonStack.Children.Add(minimizeButton);
        buttonStack.Children.Add(restoreButton);
        buttonStack.Children.Add(maximizeButton);
        buttonStack.Children.Add(closeButton);
        titleBarCustomGrid.Children.Add(appIcon);
        titleBarCustomGrid.Children.Add(title);
        titleBarCustomGrid.Children.Add(titleBarContent);
        titleBarGrid.Children.Add(titleBarCustomGrid);
        titleBarGrid.Children.Add(buttonStack);
        contentGrid.Children.Add(titleBarGrid);
        contentGrid.Children.Add(customContentGrid);
        rootGrid.Children.Add(contentGrid);
        rootGrid.Children.Add(blockingElement);
        
        Initialized += WindowOnInitialized;
        StateChanged += WindowOnStateChanged;
        Application.Current.Activated += ApplicationOnActivated;
        Application.Current.Deactivated += ApplicationOnDeactivated;
        BusyOverlay.StateChanged += BusyOverlayOnStateChanged;
        MessageBox.Shown += MessageBoxOnShown;
        MessageBox.Closed += MessageBoxOnHidden;
        return;
        
        void MessageBoxOnShown()
        {
            blockingElement.Visibility = Visibility.Visible;
            blockingElement.Focus();
            ApplicationOnDeactivated(null, EventArgs.Empty);
        }
        void MessageBoxOnHidden(MessageBoxResult result)
        {
            blockingElement.Visibility = Visibility.Hidden;
            ApplicationOnActivated(null, EventArgs.Empty);
            Activate();
        }

        void WindowOnInitialized(object? sender, EventArgs e)
        {
            if (ShowIcon && IconHelper.GetEmbeddedAppIcon() is {} source)
            {
                appIcon.Source = source;
                appIcon.Visibility = Visibility.Visible;
            }
            
            if (ShowTitle)
            {
                title.Margin = new Thickness(appIcon.Source is null ? 10 : 0, 0, 10, 0);
                title.Visibility = Visibility.Visible;
                title.Content = Title;
            }
            
            customContentGrid.Children.Add(new ContentPresenter
            {
                Content = Content
            });
            customContentGrid.Children.Add(new BusyOverlay());
            Content = rootGrid;
        }
            
        void ApplicationOnActivated(object? sender, EventArgs e)
        {
            if (blockingElement.Visibility == Visibility.Visible) return;
            minimizeButton.Foreground = foreground;
            maximizeButton.Foreground = foreground;
            restoreButton.Foreground = foreground;
            closeButton.Foreground = foreground;
            titleBarGrid.Background = white6;
            title.IsEnabled = true;
        }
            
        void ApplicationOnDeactivated(object? sender, EventArgs e)
        {
            minimizeButton.Foreground = white3;
            maximizeButton.Foreground = white3;
            restoreButton.Foreground = white3;
            closeButton.Foreground = white3;
            titleBarGrid.Background = white5;
            title.IsEnabled = false;
        }
            
        void WindowOnStateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                rootGrid.Margin = new Thickness(8);
                restoreButton.Visibility = Visibility.Visible;
                maximizeButton.Visibility = Visibility.Collapsed;
                return;
            }

            rootGrid.Margin = new Thickness(0);
            restoreButton.Visibility = Visibility.Collapsed;
            maximizeButton.Visibility = Visibility.Visible;
        }
        
        void BusyOverlayOnStateChanged(bool isBusy)
        {
            Dispatcher.Invoke(() =>
            {
                titleBarContent.IsEnabled = !isBusy;
            });
        }
    }
    
    private void MinimizeOnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void RestoreOnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Normal;
    }

    private void MaximizeOnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Maximized;
    }

    private void CloseOnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}