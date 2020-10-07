using System;

namespace OverlayApplication
{
    class GameData : ThreadedComponent
    {
        protected override string ThreadName => nameof(GameData);

        private GameProcess GameProcess { get; set; }

        public GameData(GameProcess gameProcess)
        {
            GameProcess = gameProcess;
        }

        public override void Dispose()
        {
            base.Dispose();

            GameProcess = default;
        }

        protected override void FrameAction()
        {
            if (GameProcess.IsValid)
            {
                //player.Update(GameProcess);
                Console.WriteLine("player updating");
            }
        }
    }
}
