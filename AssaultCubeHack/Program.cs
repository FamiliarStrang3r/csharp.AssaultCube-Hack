using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int key);

        [DllImport("user32")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport("kernel32")]
        static extern IntPtr GetConsoleWindow();

        const int SWP_NOZORDER = 0x4;
        const int SWP_NOACTIVATE = 0x10;

        public static void SetWindowPosition(int x, int y, int width, int height)
        {
            SetWindowPos(GetConsoleWindow(), IntPtr.Zero, x, y, width, height, SWP_NOZORDER | SWP_NOACTIVATE);
        }

        //

        public const float Rad2Deg = 180 / (float)Math.PI;
        private static int Delay = 100;
        private static bool holdingDown = false;

        private static VAMemory vam = null;
        private static string processName = "ac_client";

        private static Matrix ViewMatrix;
        private static Entity focusedTarget = null;
        private static Entity player = new Entity(Offsets.LocalPlayer);
        private static List<Entity> enemies = new List<Entity>();

        private static IntPtr handle = IntPtr.Zero;

        static void Main(string[] args)
        {
            Console.WriteLine("CONSOLE LAUNCHED!");

            int w = 1366;
            int h = 768;
            SetWindowPosition(w - 400, 100, 300, 400);

            AttachToGameProcess();

            Console.WriteLine(handle);

            Console.ReadLine();

            //ViewMatrix = ReadMatrix();

            //Console.SetWindowPosition(0, 100);
            //Console.SetWindowSize(30, 30);//min: w = 170, h = 44

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

            //InitializeEnemies();

            //Console.ReadKey();

            Console.WriteLine("Hack started");

            while (true)
            {
                EntityLoop();
                InputLoop();
                HackLoop();
                MatrixCalculations();
                Thread.Sleep(Delay);
            }
        }

        private static void EntityLoop()
        {
            int count = vam.ReadInt32((IntPtr)Offsets.PlayersCount);

            int offset = vam.ReadInt32((IntPtr)Offsets.EntityList);

            enemies.Clear();

            player.Calculate();

            string t = player.team + "/";

            for (int i = 1; i < count; i++)
            {
                Entity e = new Entity(offset + i * 0x04);
                e.Init(vam);
                e.Calculate();
                enemies.Add(e);

                t += i + ":" + e.name + ":" + e.team + " / ";
            }

            //Console.WriteLine(t);

            //Console.WriteLine($"{count} / {offset} / {enemies[0].health}");
        }

        private static void InputLoop()
        {
            holdingDown = GetAsyncKeyState(02) != 0;

            //Console.WriteLine(holdingDown);
        }

        private static void HackLoop()
        {
            //TODO: REWORK

            if (holdingDown)
            {
                if (focusedTarget != null)
                {
                    //focusedTarget.Calculate();

                    if (focusedTarget.health > 0)
                    {
                        AimTowads(focusedTarget.position);
                    }
                    else
                    {
                        focusedTarget = null;
                    }

                }
                //else
                {
                    Entity[] alive = enemies.Where(e => e.health > 0 && e.team != player.team).ToArray();

                    //if (alive.Length > 0)
                    {
                        focusedTarget = GetClosestEntity(alive);
                    }
                    //else
                    //{
                    //    focusedTarget = null;
                    //}
                }

            }
            else
            {
                focusedTarget = null;
            }

            //Console.WriteLine(enemies.Contains(focusedTarget));
        }

        private static void MatrixCalculations()
        {
            ViewMatrix = ReadMatrix();

            var t = enemies[0];

            if (t != null)
            {
                if (ViewMatrix.WorldToScreen(t.position, 800, 600, out var pos))
                {
                    Console.WriteLine(pos.ToString());
                }
                else
                {
                    Console.WriteLine("outside");
                }
            }

            Console.WriteLine(ViewMatrix.ToString());
            
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, [In, Out] byte[] lpBuffer, ulong dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwAccess, bool inherit, int pid);

        public static IntPtr OpenProcess(int pId)
        {
            handle = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, pId);
            return handle;
        }

        public static uint PROCESS_VM_READ = 0x0010;
        public static uint PROCESS_VM_WRITE = 0x0020;
        public static uint PROCESS_VM_OPERATION = 0x0008;

        public static bool GetProcessesByName(string pName, out Process process)
        {
            Process[] pList = Process.GetProcessesByName(pName);
            process = pList.Length > 0 ? pList[0] : null;
            return process != null;
        }

        private static void AttachToGameProcess()
        {
            //Visible = false;
            int count = 0;
            bool success = false;
            do
            {
                //check if game is running
                if (GetProcessesByName(processName, out var process))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();
                    Console.WriteLine("Process found: " + process.Id + ": " + process.ProcessName);
                    Console.WriteLine("Attaching...");

                    //try to attach to game process
                    try
                    {
                        //success  
                        IntPtr handle = OpenProcess(process.Id);
                        if (handle != IntPtr.Zero)
                        {
                            success = true;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Attached Handle: " + handle);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Could not attach");
                        }
                    }
                    catch (Exception ex)
                    {
                        //fail
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Attach failed: " + ex.Message);
                        Console.ReadKey(true);
                    }
                }
                else
                {
                    //process not found
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (count++ == 0)
                    {
                        Console.Clear();
                        Console.Write($"Waiting for {processName}");
                    }
                    else if (count < 10)
                    {
                        Console.Write(".");
                    }
                    else
                    {
                        count = 0;
                    }
                    Thread.Sleep(1000);
                }
            } while (!success);

            //InitializeOverlayWindowAttributes();
            //StartThreads();
        }

        private static Matrix ReadMatrix()
        {
            byte[] buffer = new byte[16 * 4];

            //read memory into buffer
            //ReadProcessMemory(handle, Offsets.ViewMatrix, buffer, (ulong)buffer.Length, out var bytesRead);

            buffer = vam.ReadByteArray((IntPtr)Offsets.ViewMatrix, (uint)buffer.Length);

            Matrix m = new Matrix();
            //convert bytes to floats
            //m.m11 = BitConverter.ToSingle(buffer, (0 * 4));
            //m.m12 = BitConverter.ToSingle(buffer, (1 * 4));
            //m.m13 = BitConverter.ToSingle(buffer, (2 * 4));
            //m.m14 = BitConverter.ToSingle(buffer, (3 * 4));

            //m.m21 = BitConverter.ToSingle(buffer, (4 * 4));
            //m.m22 = BitConverter.ToSingle(buffer, (5 * 4));
            //m.m23 = BitConverter.ToSingle(buffer, (6 * 4));
            //m.m24 = BitConverter.ToSingle(buffer, (7 * 4));

            //m.m31 = BitConverter.ToSingle(buffer, (8 * 4));
            //m.m32 = BitConverter.ToSingle(buffer, (9 * 4));
            //m.m33 = BitConverter.ToSingle(buffer, (10 * 4));
            //m.m34 = BitConverter.ToSingle(buffer, (11 * 4));

            //m.m41 = BitConverter.ToSingle(buffer, (12 * 4));
            //m.m42 = BitConverter.ToSingle(buffer, (13 * 4));
            //m.m43 = BitConverter.ToSingle(buffer, (14 * 4));
            //m.m44 = BitConverter.ToSingle(buffer, (15 * 4));

            m.m11 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 0);
            m.m12 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 4);
            m.m13 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 8);
            m.m14 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 12);

            m.m21 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 16);
            m.m22 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 20);
            m.m23 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 24);
            m.m24 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 28);

            m.m31 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 32);
            m.m32 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 36);
            m.m33 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 40);
            m.m34 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 44);

            m.m41 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 48);
            m.m42 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 52);
            m.m43 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 56);
            m.m44 = vam.ReadFloat((IntPtr)Offsets.ViewMatrix + 60);

            return m;
        }

        //

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
            if (alive.Length == 0)
                return null;

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

            //yaw = (float)Math.Atan2(dir.z, dir.x) * Rad2Deg + 90;

            //Console.WriteLine(pitch + " : " + yaw);
            player.SetRotation(pitch, yaw);
        }
    }
}
