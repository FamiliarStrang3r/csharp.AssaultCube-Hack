using System;
using System.Runtime.InteropServices;

namespace OverlayApplication
{
    class Dwmapi
    {
        [DllImport("dwmapi.dll", SetLastError = true)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);
    }
}
