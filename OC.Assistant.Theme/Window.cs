using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shell;
using Microsoft.Win32;
using OC.Assistant.Theme.Internals;

namespace OC.Assistant.Theme;

/// <inheritdoc />
/// <remarks>
/// Implements a custom TitleBar, a <see cref="Modal"/> and a <see cref="BusyOverlay"/>.<br/><br/>
/// Content inside the TitleBar can be rendered by setting the <see cref="TitleBarContent"/>.<br/><br/>
/// To show the <see cref="Modal"/>, call
/// <see cref="Modal.Show(string, UIElement, MessageBoxButton, MessageBoxImage)"/> or
/// <see cref="Modal.Show(string, string, MessageBoxButton, MessageBoxImage)"/>
/// anywhere in your application.<br/><br/>
/// To activate/deactivate the <see cref="BusyOverlay"/>, call<br/>
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
    }
    
    private void InitializeComponent()
    {
        SetResourceReference(StyleProperty, "DefaultWindowStyle");
        
        var titleBarHeight = (double)FindResource("TitleBarHeight");
        var background = (SolidColorBrush)FindResource("BackgroundBaseBrush");
        var foreground = (SolidColorBrush)FindResource("ForegroundBaseBrush");
        var white4 = (SolidColorBrush)FindResource("White4Brush");
        var white5 = (SolidColorBrush)FindResource("White5Brush");
        var white6 = (SolidColorBrush)FindResource("White6Brush");
        
        var buildNumber = Registry.GetValue(
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", 
            "CurrentBuild", null) as string;
        
        var isWindows11 = int.TryParse(buildNumber, out var build) && build >= 22000;
        
        var chrome = new WindowChrome
        {
            CaptionHeight = titleBarHeight,
            ResizeBorderThickness = new Thickness(6),
            GlassFrameThickness = new Thickness(0),
            CornerRadius = new CornerRadius(isWindows11 ? 12 : 0) 
        };
        WindowChrome.SetWindowChrome(this, chrome);
        
        var rootBorder = new Border
        {
            BorderBrush = background,
            BorderThickness = new Thickness(0)
        };

        var rootGrid = new Grid();
        
        var blurEffect = new BlurEffect
        {
            Radius = 0
        };
        
        var contentBorder = new Border
        {
            Effect = blurEffect
        };
        
        var contentGrid = new Grid
        {
            Background = background,
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
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
            }
        };
        
        var appIcon = new Image
        {
            Visibility = Visibility.Collapsed,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Margin = new Thickness(10, 8, 10, 8)
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
        
        var modal = new Modal
        {
            BlurEffect = blurEffect
        };
        WindowChrome.SetIsHitTestVisibleInChrome(modal, true);
        
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
        contentBorder.Child = contentGrid;
        rootGrid.Children.Add(contentBorder);
        rootGrid.Children.Add(modal);
        rootBorder.Child = rootGrid;
        
        Initialized += WindowOnInitialized;
        StateChanged += WindowOnStateChanged;
        Application.Current.Activated += ApplicationOnActivated;
        Application.Current.Deactivated += ApplicationOnDeactivated;
        BusyOverlay.StateChanged += BusyOverlayOnStateChanged;
        return;

        void WindowOnInitialized(object? sender, EventArgs e)
        {
            if (ShowIcon)
            {
                appIcon.Visibility = Visibility.Visible;
                appIcon.Source = IconHelper.GetEmbeddedAppIcon();
            }
            
            if (ShowTitle)
            {
                title.Margin = new Thickness(ShowIcon ? 0 : 10, 0, 0, 0);
                title.Visibility = Visibility.Visible;
                title.Content = Title;
            }
            
            customContentGrid.Children.Add(new ContentPresenter
            {
                Content = Content
            });
            customContentGrid.Children.Add(new BusyOverlay());
            Content = rootBorder;
        }
            
        void ApplicationOnActivated(object? sender, EventArgs e)
        {
            minimizeButton.Foreground = foreground;
            maximizeButton.Foreground = foreground;
            restoreButton.Foreground = foreground;
            closeButton.Foreground = foreground;
            titleBarGrid.Background = white6;
            title.IsEnabled = true;
        }
            
        void ApplicationOnDeactivated(object? sender, EventArgs e)
        {
            minimizeButton.Foreground = white4;
            maximizeButton.Foreground = white4;
            restoreButton.Foreground = white4;
            closeButton.Foreground = white4;
            titleBarGrid.Background = white5;
            title.IsEnabled = false;
        }
            
        void WindowOnStateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                rootBorder.BorderThickness = new Thickness(8);
                restoreButton.Visibility = Visibility.Visible;
                maximizeButton.Visibility = Visibility.Collapsed;
                return;
            }

            rootBorder.BorderThickness = new Thickness(0);
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