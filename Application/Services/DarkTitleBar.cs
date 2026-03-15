using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace UmweltMonitor3000.Application.Services;
public static class DarkTitleBar
{
    // Windows 10 1809: 19, ab 1903/Win11: 20
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    public static bool TrySetDarkTitleBar(Window window, bool enabled = true)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
            return false;

        int useDark = enabled ? 1 : 0;
        int attribute = GetImmersiveDarkModeAttributeId();

        // Versuch 1: Neuerer Attributwert
        int result = DwmSetWindowAttribute(hwnd, attribute, ref useDark, sizeof(int));
        if (result == 0)
            return true;

        // Versuch 2: Älterer Attributwert (Fallback)
        attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
        result = DwmSetWindowAttribute(hwnd, attribute, ref useDark, sizeof(int));
        return result == 0;
    }

    private static int GetImmersiveDarkModeAttributeId()
    {
        // Ab Windows 10 1903 (Build 18362) ist 20 korrekt
        Version os = Environment.OSVersion.Version; // 10.0.x
                                                    // Windows 10 1903 = 10.0.18362
        bool is1903OrNewer = (os.Major > 10) || (os.Major == 10 && (os.Build >= 18362));
        return is1903OrNewer ? DWMWA_USE_IMMERSIVE_DARK_MODE : DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
    }
}
