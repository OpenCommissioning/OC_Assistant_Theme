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
        IntPtr[] pIconLarge,
        IntPtr[] pIconSmall,
        uint nIcons);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    /// <summary>
    /// Retrieves the application's embedded icon from its executable file.
    /// </summary>
    /// <param name="smallIcon">
    /// A boolean indicating whether to retrieve the small version of the icon.
    /// </param>
    /// <returns>
    /// The application's embedded icon as an <see cref="ImageSource"/>, if any.
    /// </returns>
    public static ImageSource? GetEmbeddedAppIcon(bool smallIcon = false)
    {
        var exePath = Assembly.GetEntryAssembly()?.Location;
        if (exePath is null) return null;
        
        var largeIcons = new IntPtr[1];
        var smallIcons = new IntPtr[1];

        var iconsExtracted = ExtractIconEx(exePath, 0, largeIcons, smallIcons, 1);
        if (iconsExtracted <= 0) return null;
        
        var iconHandle = smallIcon ? smallIcons[0] : largeIcons[0];
        if (iconHandle == IntPtr.Zero) return null;
        
        ImageSource image = Imaging.CreateBitmapSourceFromHIcon(
            iconHandle,
            System.Windows.Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        DestroyIcon(iconHandle);
        return image;
    }
}