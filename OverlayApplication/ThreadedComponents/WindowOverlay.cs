using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;

namespace OverlayApplication
{
    public class WindowOverlay : ThreadedComponent
    {
        //Color c = Color.Gray;

        protected override int ThreadTimeout { get; set; } = 500;

        protected GameProcess GameProcess { get; set; }

        public Form Window { get; private set; }

        public WindowOverlay(GameProcess gameProcess)
        {
            GameProcess = gameProcess;

            Window = new Form()
            {
                Name = "Overlay Name",
                Text = "Overlay Text",
                MinimizeBox = false,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.None,
                TopMost = true,
                //Width = 16,
                //Height = 16,
                //Left = -32000,
                //Top = -32000,
                //StartPosition = FormStartPosition.Manual

                //BackColor = c,
                //TransparencyKey = c
            };

            Window.Load += (sender, args) =>
            {
                var exStyle = User32.GetWindowLong(Window.Handle, User32.GWL_EXSTYLE);
                exStyle |= User32.WS_EX_LAYERED;
                exStyle |= User32.WS_EX_TRANSPARENT;

                // make the window's border completely transparent
                User32.SetWindowLong(Window.Handle, User32.GWL_EXSTYLE, (IntPtr)exStyle);

                // set the alpha on the whole window to 255 (solid)
                User32.SetLayeredWindowAttributes(Window.Handle, 0, 255, User32.LWA_ALPHA);
            };

            Window.SizeChanged += (sender, args) => ExtendFrameIntoClientArea();
            Window.LocationChanged += (sender, args) => ExtendFrameIntoClientArea();
            Window.Closed += (sender, args) => System.Windows.Application.Current.Shutdown();

            Window.Show();
        }

        public override void Dispose()
        {
            base.Dispose();

            Window.Close();
            Window.Dispose();
            Window = default;

            GameProcess = default;
        }

        private void ExtendFrameIntoClientArea()
        {
            var margins = new Margins
            {
                Left = -1,
                Right = -1,
                Top = -1,
                Bottom = -1,
            };
            Dwmapi.DwmExtendFrameIntoClientArea(Window.Handle, ref margins);
        }

        protected override void FrameAction()
        {
            Update(GameProcess.WindowRectangleClient);
        }

        private void Update(Rectangle windowRectangleClient)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // TODO: temporary
                //Window.BackColor = c; 

                if (Window.Location != windowRectangleClient.Location || Window.Size != windowRectangleClient.Size)
                {
                    if (windowRectangleClient.Width > 0 && windowRectangleClient.Height > 0)
                    {
                        // valid
                        Window.Location = windowRectangleClient.Location;
                        Window.Size = windowRectangleClient.Size;
                    }
                    else
                    {
                        // invalid
                        Window.Location = new System.Drawing.Point(-32000, -32000);
                        Window.Size = new Size(16, 16);
                    }
                }
            }, DispatcherPriority.Normal);
        }
    }
}
