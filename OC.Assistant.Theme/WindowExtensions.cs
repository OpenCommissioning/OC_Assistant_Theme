using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OC.Assistant.Theme;

/// <summary>
/// Provides extension methods for enhancing the behavior of a <see cref="System.Windows.Window"/>.
/// </summary>
internal static class WindowExtensions
{
    private const int WM_SYS_COMMAND = 0x112;
    
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr handle, int attr, ref bool attrValue, int attrSize);
    
    private static IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != WM_SYS_COMMAND) return IntPtr.Zero;
        handled = MessageBox.IsOpen;
        return IntPtr.Zero;
    }
    
    /// <summary>
    /// Sets an attribute via <see cref="DwmSetWindowAttribute"/> to change the non-client area to dark.
    /// </summary>
    /// <param name="window">The <see cref="System.Windows.Window"/> instance to which the attribute is set.</param>
    /// <remarks>Is used to avoid white flickering while opening or resizing the window.</remarks>
    public static void SetDarkAttribute(this System.Windows.Window window)
    {
        window.Loaded += WindowOnLoaded;
        return;
        
        void WindowOnLoaded(object o, RoutedEventArgs routedEventArgs)
        {
            var value = true;
            var handle = new WindowInteropHelper(window).Handle;
            _ = DwmSetWindowAttribute(handle, 20, ref value, Marshal.SizeOf(value));
        }
    }

    /// <summary>
    /// Adds a hook to the message loop of the specified <see cref="System.Windows.Window"/>.
    /// </summary>
    /// <param name="window">The <see cref="System.Windows.Window"/> instance to which the hook is added.</param>
    /// <remarks>Is used to ignore <see cref="WM_SYS_COMMAND"/> messages like Resize,
    /// Maximize and Minimize when the <see cref="MessageBox"/> is open.</remarks>
    public static void AddMessageBoxHook(this System.Windows.Window window)
    {
        window.Loaded += WindowOnLoaded;
        return;
        
        void WindowOnLoaded(object o, RoutedEventArgs routedEventArgs)
        {
            var handle = new WindowInteropHelper(window).Handle;
            HwndSource.FromHwnd(handle)?.AddHook(WndProc);
        }
    }
}