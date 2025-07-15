using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OC.Assistant.Theme.Internals;

/// <summary>
/// Provides methods for handling application icons.
/// </summary>
public static class IconHelper
{
    [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
    private static extern uint ExtractIconEx(
        string lpszFile,
        int nIconIndex,
        IntPtr[]? pIconLarge,
        IntPtr[]? pIconSmall,
        uint nIcons);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    /// <summary>
    /// Retrieves the application's embedded icon from its executable file.
    /// </summary>
    /// <returns>
    /// The application's embedded icon as an <see cref="ImageSource"/>, if any.
    /// </returns>
    public static ImageSource? GetEmbeddedAppIcon()
    {
        var exePath = Assembly.GetEntryAssembly()?.Location;
        if (exePath is null) return null;
        
        var largeIcons = new IntPtr[1];
        var iconsExtracted = ExtractIconEx(exePath, 0, largeIcons, null, 1);
        if (iconsExtracted <= 0) return null;
        
        if (largeIcons[0] == IntPtr.Zero) return null;
        
        ImageSource image = Imaging.CreateBitmapSourceFromHIcon(
            largeIcons[0],
            System.Windows.Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
        
        DestroyIcon(largeIcons[0]);
        return image;
    }
}