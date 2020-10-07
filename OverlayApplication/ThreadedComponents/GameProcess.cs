using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace OverlayApplication
{
    public class GameProcess : ThreadedComponent
    {
        private const string NAME_PROCESS = "ac_client";
        private const string NAME_WINDOW = "AssaultCube";

        protected override string ThreadName => nameof(GameProcess);
        protected override int ThreadFrameSleep { get; set; } = 500;

        public Process Process { get; private set; }
        private IntPtr WindowHwnd { get; set; }
        public Rectangle WindowRectangleClient { get; private set; }
        private bool WindowActive { get; set; }

        public bool IsValid => WindowActive && !(Process is null);

        public override void Dispose()
        {
            InvalidateWindow();
            InvalidateModules();
            base.Dispose();
        }

        protected override void FrameAction()
        {
            if (!EnsureProcessAndModules())
            {
                InvalidateModules();
            }

            if (!EnsureWindow())
            {
                InvalidateWindow();
            }

            string text = IsValid ? $"0x{(int)Process.Handle:X8} {WindowRectangleClient.X} {WindowRectangleClient.Y} {WindowRectangleClient.Width} {WindowRectangleClient.Height}" : "Game process invalid";
            text = IsValid ? WindowRectangleClient.ToString() : "Invalid";
            Console.WriteLine(text);
        }

        private void InvalidateWindow()
        {
            WindowHwnd = IntPtr.Zero;
            WindowRectangleClient = Rectangle.Empty;
            WindowActive = false;
        }

        private void InvalidateModules()
        {
            Process?.Dispose();
            Process = default;
        }

        private bool EnsureWindow()
        {
            WindowHwnd = User32.FindWindow(null, NAME_WINDOW);
            if (WindowHwnd == IntPtr.Zero)
            {
                return false;
            }

            WindowRectangleClient = U.GetClientRectangle(WindowHwnd);
            if (WindowRectangleClient.Width <= 0 || WindowRectangleClient.Height <= 0)
            {
                return false;
            }

            WindowActive = WindowHwnd == User32.GetForegroundWindow();

            return WindowActive;
        }

        private bool EnsureProcessAndModules()
        {
            if (Process is null)
            {
                Process = Process.GetProcessesByName(NAME_PROCESS).FirstOrDefault();
            }

            if (Process is null || !Process.IsRunning())
            {
                return false;
            }

            //if (ModuleClient is null)
            //{
            //    ModuleClient = Process.GetModule(NAME_MODULE_CLIENT);
            //}
            //if (ModuleClient is null)
            //{
            //    return false;
            //}

            //if (ModuleEngine is null)
            //{
            //    ModuleEngine = Process.GetModule(NAME_MODULE_ENGINE);
            //}
            //if (ModuleEngine is null)
            //{
            //    return false;
            //}

            return true;
        }
    }
}
