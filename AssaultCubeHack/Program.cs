using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

//AssaultCube v1.2.0.2

namespace AssaultCubeHack
{
    class Program
    {
        //0x00509B74 - comments (works)
        //0x004E4DBC - original (outdated)

        //public static int Base = 0x50F4F4;
        //public static int Health = 0xF8;//0xF4;

        public const float Rad2Deg = 180 / (float)Math.PI;
        private static int Delay = 10;

        private static VAMemory vam = null;
        private static string processName = "ac_client";

        private static Entity player = new Entity(0x50F4F4);
        private static List<Entity> enemies = new List<Entity>();

        static void Main(string[] args)
        {
            Console.WriteLine("CONSOLE LAUNCHED!");
            //Console.SetWindowPosition(0, 0);

            MyHack();

            Console.ReadKey();
        }

        //private static void Hack()
        //{
        //    VAMemory vam = new VAMemory("ac_client");
        //    int LocalPlayer = vam.ReadInt32((IntPtr)Base);
        //    while (true)
        //    {
        //        int address = LocalPlayer + Health;
        //        vam.WriteInt32((IntPtr)address, 777);
        //        Thread.Sleep(100);
        //    }
        //}

        private static void MyHack()
        {
            vam = new VAMemory(processName);

            player.Init(vam);

            player.SetHealth(150);

            InitializeEnemies();

            Console.ReadKey();

            Console.WriteLine("Hack started");

            while (true)
            {
                HackLoop();
                Thread.Sleep(Delay);
            }
        }

        private static void HackLoop()
        {
            player.Calculate();

            for (int i = 0, length = enemies.Count; i < length; i++)
            {
                enemies[i].Calculate();
                //Console.WriteLine(enemies[i].position.ToString());
            }

            //Console.WriteLine();

            //var target = enemies[0];
            //Console.WriteLine(target.position);


            Entity[] alive = enemies.Where(e => e.health > 0).ToArray();

            if (alive.Length > 0)
            {
                Entity target = GetClosestEntity(alive);
                AimTowads(target.position);
            }
        }

        private static void InitializeEnemies()
        {
            int offset = vam.ReadInt32((IntPtr)Offsets.EntityList);
            Console.WriteLine(offset.ToString("X"));

            //int total = 128;//32 players, 4 byte per player

            int players = 4;
            int counter = 0;

            for (int i = 4; i < players * 4; i += 4)
            {
                /*
                int enemyOffset = vam.ReadInt32((IntPtr)(offset + i));

                Entity e = new Entity();
                e.Init(vam);
                e.baseAddress = enemyOffset;

                enemies.Add(e);

                e.Calculate();
                */

                //new
                Entity e = new Entity(offset + i);
                e.Init(vam);
                enemies.Add(e);
                e.Calculate();
                //

                //was good
                counter++;
                int health = -1;
                //health = vam.ReadInt32((IntPtr)(enemyOffset + Offsets.HealthOffset));
                //string text = $"{counter}: {health}";

                Console.WriteLine(counter + ": " + e.health + " / " + health);
            }

            Console.WriteLine();
        }

        private static Entity GetClosestEntity(Entity[] alive)
        {
            Entity candidate = alive[0];

            if (alive.Length == 1)
                return candidate;

            for (int i = 1, length = alive.Length; i < length; i++)
            {
                var e = alive[i];

                float distance = Vector3.Distance(player.position, e.position);

                if (distance < Vector3.Distance(player.position, candidate.position))
                {
                    candidate = e;
                }
            }

            return candidate;
        }

        private static void AimTowads(Vector3 position)
        {
            Vector3 dir = position - player.position;
            float distance = Vector3.Distance(player.position, position);

            float pitch = (float)Math.Asin(dir.y / distance) * Rad2Deg;
            float yaw = -(float)Math.Atan2(dir.z, dir.x) * Rad2Deg + 180;

            //Console.WriteLine(pitch + " : " + yaw);
            player.SetRotation(pitch, yaw);
        }
    }
}
