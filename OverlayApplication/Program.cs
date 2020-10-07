using System;
using System.Windows;

namespace OverlayApplication
{
    public class Program : Application, IDisposable
    {
        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }
        private WindowOverlay WindowOverlay { get; set; }
        private Graphics Graphics { get; set; }

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
            Console.WriteLine("LAUNCHED");

            GameProcess = new GameProcess();
            GameData = new GameData(GameProcess);
            WindowOverlay = new WindowOverlay(GameProcess);
            Graphics = new Graphics(WindowOverlay, GameProcess, GameData);

            GameProcess.Start();
            GameData.Start();
            WindowOverlay.Start();
            Graphics.Start();

            Console.WriteLine("Program ctor end");
        }

        public void Dispose()
        {
            Graphics.Dispose();
            Graphics = default;

            WindowOverlay.Dispose();
            WindowOverlay = default;

            GameData.Dispose();
            GameData = default;

            GameProcess.Dispose();
            GameProcess = default;
        }
    }
}
