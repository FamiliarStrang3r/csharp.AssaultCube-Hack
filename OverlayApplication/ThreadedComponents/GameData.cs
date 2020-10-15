using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.DirectX;

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

        protected override int ThreadFrameSleep { get; set; } = 20;

        private GameProcess GameProcess { get; set; }

        //
        public Matrix ViewMatrix { get; private set; }
        public Entity Player { get; private set; } = new Entity(Offsets.LocalPlayer);
        public List<Entity> Bots { get; private set; } = new List<Entity>();
        //

        public bool IsDM { get; private set; } = false;

        private Vector3 BasePosition;
        private Vector3 FlagPosition;

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

            Player.Update(GameProcess);

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

            //GameProcess.Process.Write((IntPtr)Offsets.LocalPlayer + 0x150, 100);
            //int admin = GameProcess.Process.Read<int>((IntPtr)Offsets.LocalPlayer + 0x150);// + 0x220);
            //Console.WriteLine(admin);

            Entity[] alive = null;

            if (IsDM)
            {
                alive = Bots.Where(e => e.Health > 0).ToArray();
            }
            else
            {
                alive = Bots.Where(e => e.Health > 0 && e.Team != Player.Team).ToArray();
            }

            var target = GetClosestEntity(alive);

            bool holdingDown = User32.GetAsyncKeyState(Keys.RButton) != 0;

            if (target != null && holdingDown)
            {
                Player.AimTowards(GameProcess, target);
                //Console.WriteLine(Player.Pitch + " / " + Player.Yaw);
            }

            bool clicked = User32.GetAsyncKeyState(Keys.Q) != 0;

            if (clicked)
            {
                IsDM = !IsDM;
                Console.WriteLine("DM: " + IsDM);
                System.Threading.Thread.Sleep(100);
            }

            Teleport();
        }

        private void Teleport()
        {
            if (User32.GetAsyncKeyState(Keys.NumPad1) != 0)
            {
                BasePosition = Player.FootPosition;
            }
            else if (User32.GetAsyncKeyState(Keys.NumPad2) != 0)
            {
                Player.SetPosition(GameProcess, BasePosition + new Vector3(0, 0, 1));
            }

            if (User32.GetAsyncKeyState(Keys.NumPad4) != 0)
            {
                FlagPosition = Player.FootPosition;
            }
            else if (User32.GetAsyncKeyState(Keys.NumPad5) != 0)
            {
                Player.SetPosition(GameProcess, FlagPosition + new Vector3(0, 0, 1));
            }
        }

        private Entity GetClosestEntity(Entity[] alive)
        {
            if (alive.Length == 0)
                return null;

            Entity candidate = alive[0];

            if (alive.Length == 1)
                return candidate;

            for (int i = 1, length = alive.Length; i < length; i++)
            {
                var e = alive[i];


                //float distance = Vectors.Vector3.Distance(Player.HeadPosition, e.HeadPosition);

                //if (distance < Vectors.Vector3.Distance(Player.HeadPosition, candidate.HeadPosition))
                //{
                //    candidate = e;
                //}
                var dir = Player.HeadPosition - e.HeadPosition;
                var dir2 = Player.HeadPosition - candidate.HeadPosition;

                float distance = dir.Length();
                float distance2 = dir2.Length();

                if (distance < distance2)
                {
                    candidate = e;
                }
            }

            return candidate;
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
