using System;
using System.Collections.Generic;
//using Microsoft.DirectX;

//https://vk.com/topic-9618_21661347?offset=340
//Чтобы её избежать в студии нажмите Ctrl+Alt+E.
//В появившемся окне выберите третий сверху пункт managed debugging assistance
//уберите галочку напротив loader lock.

//http://www.gamedev.ru/code/forum/?id=78766

namespace OverlayApplication
{
    public class GameData : ThreadedComponent
    {
        protected override string ThreadName => nameof(GameData);

        protected override int ThreadFrameSleep { get; set; } = 50;

        private GameProcess GameProcess { get; set; }

        //
        public Matrix ViewMatrix { get; private set; }
        private Entity player = new Entity(Offsets.LocalPlayer);
        public List<Entity> Bots { get; private set; } = new List<Entity>();
        //

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
                //ReadValues();

                EntityLoop();
            }
        }

        private void EntityLoop()
        {
            ViewMatrix = U.Read<Matrix>(GameProcess.Process.Handle, (IntPtr)Offsets.ViewMatrix);

            player.Update(GameProcess);

            int count = GameProcess.Process.Read<int>((IntPtr)Offsets.PlayersCount);
            int entityList = GameProcess.Process.Read<int>((IntPtr)Offsets.EntityList);

            Bots.Clear();

            for (int i = 1; i < count; i++)
            {
                Entity e = new Entity(entityList + i * 0x4);
                e.Update(GameProcess);
                Bots.Add(e);

                //Console.WriteLine(e.ToString());
            }

            //Console.WriteLine();
        }

        private void ReadValues()
        {
            //var localPlayer = U.Read<IntPtr>(GameProcess.Process.Handle, (IntPtr)0x50F4F4);
            //localPlayer = GameProcess.Process.Read<IntPtr>((IntPtr)0x50F4F4);
            //int hp = U.Read<int>(GameProcess.Process.Handle, (IntPtr)localPlayer + 0xF8);
            //Console.WriteLine(hp);

            var entityList = U.Read<IntPtr>(GameProcess.Process.Handle, (IntPtr)0x0050F4F8);
            var enemy = U.Read<IntPtr>(GameProcess.Process.Handle, entityList + 4);
            var hp = U.Read<int>(GameProcess.Process.Handle, (IntPtr)enemy + 0xF8);
            //Console.WriteLine(hp);
            //Vector3 pos = U.Read<Vector3>(GameProcess.Process.Handle, (IntPtr)enemy + 0x4);
            //Console.WriteLine(pos.ToString());

            //ViewMatrix = U.Read<Matrix>(GameProcess.Process.Handle, (IntPtr)Offsets.ViewMatrix);

            //bool inside = m.WorldToScreen(pos, 800, 600, out var p);
            //Console.WriteLine($"{inside}: {p}");

            Console.WriteLine();

            //var size = System.Runtime.InteropServices.Marshal.SizeOf<Matrix>();
            //var buffer = (object)default(Matrix);
            //Kernel32.ReadProcessMemory(GameProcess.Process.Handle, (IntPtr)Matrix.pointer, buffer, size, out var lpNumberOfBytesRead);
            //Matrix m4x4 = lpNumberOfBytesRead == size ? (Matrix)buffer : default;
            //Console.WriteLine(m4x4.ToString());
        }
    }
}
