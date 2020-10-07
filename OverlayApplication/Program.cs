using System;
using System.Windows;

namespace OverlayApplication
{
    public class Program : Application, IDisposable
    {
        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }
        private WindowOverlay WindowOverlay { get; set; }

        public static void Main()
        {
            new Program().Run();
        }

        public Program()
        {
            Startup += (sender, args) => Ctor();
            Exit += (sender, args) => Dispose();
        }

        public void Ctor()
        {
            GameProcess = new GameProcess();
            GameData = new GameData(GameProcess);
            WindowOverlay = new WindowOverlay(GameProcess);

            GameProcess.Start();
            GameData.Start();
            WindowOverlay.Start();
        }

        public void Dispose()
        {
            WindowOverlay.Dispose();
            WindowOverlay = default;

            GameData.Dispose();
            GameData = default;

            GameProcess.Dispose();
            GameProcess = default;
        }
    }
}
