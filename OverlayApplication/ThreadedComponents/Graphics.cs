using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using System.Windows.Threading;

namespace OverlayApplication
{
    public class Graphics : ThreadedComponent
    {
        protected override string ThreadName => nameof(Graphics);

        protected override int ThreadFrameSleep { get; set; } = 20;

        private WindowOverlay WindowOverlay { get; set; }

        public GameProcess GameProcess { get; private set; }

        public GameData GameData { get; private set; }

        //private FpsCounter FpsCounter { get; set; }

        public Device Device { get; private set; }

        public Microsoft.DirectX.Direct3D.Font FontVerdana { get; private set; }

        public Graphics(WindowOverlay windowOverlay, GameProcess gameProcess, GameData gameData)
        {
            WindowOverlay = windowOverlay;
            GameProcess = gameProcess;
            GameData = gameData;
            //FpsCounter = new FpsCounter();

            InitDevice();
            FontVerdana = new Microsoft.DirectX.Direct3D.Font(Device, new System.Drawing.Font("Verdana", 12, FontStyle.Regular));
        }

        public override void Dispose()
        {
            base.Dispose();

            FontVerdana.Dispose();
            FontVerdana = default;
            Device.Dispose();
            Device = default;

            //FpsCounter = default;
            GameData = default;
            GameProcess = default;
            WindowOverlay = default;
        }

        private void InitDevice()
        {
            var parameters = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                DeviceWindow = WindowOverlay.Window,
                MultiSampleQuality = 0,
                BackBufferFormat = Format.A8R8G8B8,
                BackBufferWidth = WindowOverlay.Window.Width,
                BackBufferHeight = WindowOverlay.Window.Height,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = DepthFormat.D16,
                PresentationInterval = PresentInterval.Immediate, // turn off v-sync
            };

            Device.IsUsingEventHandlers = true;
            Device = new Device(0, DeviceType.Hardware, WindowOverlay.Window, CreateFlags.HardwareVertexProcessing, parameters);
        }

        protected override void FrameAction()
        {
            if (!GameProcess.IsValid)
            {
                return;
            }

            //FpsCounter.Update();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // set render state
                Device.RenderState.AlphaBlendEnable = true;
                Device.RenderState.AlphaTestEnable = false;
                Device.RenderState.SourceBlend = Blend.SourceAlpha;
                Device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                Device.RenderState.Lighting = false;
                Device.RenderState.CullMode = Cull.None;
                Device.RenderState.ZBufferEnable = true;
                Device.RenderState.ZBufferFunction = Compare.Always;

                // clear scene
                Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.FromArgb(0, 0, 0, 0), 1, 0);

                // render scene
                Device.BeginScene();
                Render();
                Device.EndScene();

                // flush to screen
                Device.Present();
            }, DispatcherPriority.Normal);
        }

        private void Render()
        {
            //DrawWindowBorder();
            //DrawFps();

            int w = GameProcess.WindowRectangleClient.Width;

            var gameMode = GameData.IsDM ? "Deathmatch" : "Team Deathmatch";
            FontVerdana.DrawText(default, gameMode, (int)(w * 0.4), 0 + 10, Color.White);

            try
            {
                DrawEntities();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void DrawFps()
        {
            //FontVerdana8.DrawText(default, $"{FpsCounter.Fps:0} FPS", 5, 5, Color.Red);
        }

        private void DrawWindowBorder()
        {
            int w = GameProcess.WindowRectangleClient.Width;
            int h = GameProcess.WindowRectangleClient.Height;

            int offset = 10;

            Device.DrawPolyline(new[]
            {
                new Vector3(0, 0, 0),
                new Vector3(w - offset, 0, 0),
                new Vector3(w - offset, h - offset, 0),
                new Vector3(0, h - offset, 0),
                new Vector3(0, 0, 0)
            }, Color.LawnGreen);
        }

        private void DrawEntities()
        {
            Color enemyColor = Color.White;
            Color teamColor = GameData.IsDM ? enemyColor : Color.Green;

            int w = GameProcess.WindowRectangleClient.Width;
            int h = GameProcess.WindowRectangleClient.Height;

            for (int i = 0; i < GameData.Bots.Count; i++)
            {
                var entity = GameData.Bots[i];

                if (entity.Health <= 0) continue;

                Color color = entity.Team == GameData.Player.Team ? teamColor : enemyColor;

                if (GameData.ViewMatrix.WorldToScreen(entity.HeadPosition, w, h, out var headPosition))
                {
                    int offset = 10;

                    if (GameData.ViewMatrix.WorldToScreen(entity.FootPosition, w, h, out var footPosition))
                    {
                        float height = Math.Abs(headPosition.Y - footPosition.Y);
                        float width = height / 3;

                        Device.DrawPolyline(new Vector3[]
                        {
                            new Vector3(headPosition.X - width, headPosition.Y - offset, 0),
                            new Vector3(headPosition.X + width, headPosition.Y - offset, 0),
                            new Vector3(footPosition.X + width, footPosition.Y, 0),
                            new Vector3(footPosition.X - width, footPosition.Y, 0),
                            new Vector3(headPosition.X - width, headPosition.Y - offset, 0),
                        }, color);
                    }

                    string upperText = $"{entity.Health} : {entity.Armor}";
                    FontVerdana.DrawText(default, upperText, (int)headPosition.X, (int)headPosition.Y - offset * 3, color);

                    Device.DrawPolyline(new Vector3[]
                    {
                        new Vector3(w * 0.5f, h, 0),
                        footPosition
                    }, color);
                }
            }
        }
    }
}
