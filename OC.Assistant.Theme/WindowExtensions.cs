using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OC.Assistant.Theme;

/// <summary>
/// Provides extension methods for enhancing the behavior of a <see cref="System.Windows.Window"/>.
/// </summary>
internal static class WindowExtensions
{
    ///<summary>
    /// Specifies the message sent to a window when a menu is about to be displayed.
    ///</summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/wm-initmenu">
    /// microsoft documentation</see>
    /// </remarks>
    private const int WM_INITMENU = 0x0116;
    
    ///<summary>
    /// Specifies the message to cancel certain modes, such as mouse capture.
    ///</summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-cancelmode">
    /// microsoft documentation</see>
    /// </remarks>
    private const int WM_CANCELMODE = 0x001F;
    
    ///<summary>
    /// Specifies the message when the user chooses a command from the Window menu.
    ///</summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/de-de/windows/win32/menurc/wm-syscommand">
    /// microsoft documentation</see>
    /// </remarks>
    private const int WM_SYSCOMMAND = 0x0112;
    
    ///<summary>
    /// Specifies the parameter to minimize the window.
    ///</summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/de-de/windows/win32/menurc/wm-syscommand">
    /// microsoft documentation</see>
    /// </remarks>
    private const int SC_MINIMIZE = 0xF020;
    
    /// <summary>
    /// Sets the value of Desktop Window Manager (DWM) non-client rendering attributes for a window.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmsetwindowattribute">
    /// microsoft documentation</see>
    /// </remarks>
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr handle, int attr, ref bool attrValue, int attrSize);
    
    /// <summary>
    /// Sends the specified message to a window or windows.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessage">
    /// microsoft documentation</see>
    /// </remarks>
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    
    /// <summary>
    /// Represents the method that handles Win32 window messages.
    /// This implementation cancels the title bar's context menu and prevents the application from getting minimized
    /// when the <see cref="MessageBox"/> is open.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-wndproc">
    /// microsoft documentation</see>
    /// </remarks>
    private static IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM_INITMENU:
                SendMessage(hWnd, WM_CANCELMODE, IntPtr.Zero, IntPtr.Zero);
                handled = true;
                break;
            case WM_SYSCOMMAND: 
                var sc = (int)(wParam.ToInt64() & 0xFFF0);
                if (sc == SC_MINIMIZE)
                {
                    handled = true;
                }
                break;
        }
        
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
    /// Disables the title bar's context menu.
    /// </summary>
    /// <param name="window">The <see cref="System.Windows.Window"/> instance to which the hook is added.</param>
    /// <remarks>Adds a hook to the window's message loop.</remarks>
    public static void DisableContextMenu(this System.Windows.Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;
        HwndSource.FromHwnd(handle)?.AddHook(WndProc);
    }
    
    /// <summary>
    /// Re-enables the title bar's context menu if it has been disabled via <see cref="DisableContextMenu"/>.
    /// </summary>
    /// <param name="window">The <see cref="System.Windows.Window"/> instance from which the hook is removed.</param>
    /// <remarks> Removes a hook from the window's message loop.</remarks>
    public static void EnableContextMenu(this System.Windows.Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;
        HwndSource.FromHwnd(handle)?.RemoveHook(WndProc);
    }
}