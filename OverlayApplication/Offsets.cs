using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverlayApplication
{
    public static class Offsets
    {
        public const int LocalPlayer = 0x50F4F4;
        public const int EntityList = 0x50F4F8;
        public const int PlayersCount = 0x50F500;

        public const int ViewMatrix = 0x501AE8;

        public const int HeadPosition = 0x4;
        public const int FootPosition = 0x34;

        public const int Team = 0x32C;
        public const int Name = 0x0225;
        public const int Health = 0xF8;
        public const int Armor = 0xFC;

        public const int Yaw = 0x40;    //[0..360]
        public const int Pitch = 0x44;  //[-90..90]
        public const int Roll = 0x48;

        public const int KillCount = 0x1FC;
        public const int DeathCount = 0x204;

        //ac_client.exe + FCB08
    }

    //https://guidedhacking.com/threads/aimbot-smooth.5645/
    //return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);

    
}
