using System.Runtime.InteropServices;

namespace OverlayApplication
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left, Top, Right, Bottom;
    }
}
