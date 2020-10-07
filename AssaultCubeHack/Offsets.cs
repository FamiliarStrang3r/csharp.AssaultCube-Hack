
namespace AssaultCubeHack
{
    static class Offsets
    {
        //0x0050F4F8 array pointer
        // + 0x4 bot offset
        // + 0xF8 bot health offset

        public const int LocalPlayer = 0x50F4F4;
        public const int EntityList = 0x0050F4F8;
        public const int ViewMatrix = 0x00501AE8;

        public const int PlayersCount = 0x0050F500;//0x0050F4E8 + 0x18;

        public const int Name = 0x0225;//224 works too
        public const int Team = 0x032C;

        public const int Health = 0xF8;
        public const int Armor = 0xFC;

        public const int PistolTotalAmmo = 0x114;
        public const int PistolClipAmmo = 0x13C;

        //not recoil - reload time
        //pistol recoil = 0x164
        //mp5 recoil = 0x170
        //rifle 0x178
        //akimbo recoil = 0x184
        
        //akimbo 0x10C = bool: 1 true, 0 false
        //total ammo akimbo 0x134
        //clip ammo akimbo 0x15C

        //grenadeCount = 0x148
        //public const int RifleTotalAmmo = 0x120;// - ,mp5 // rifle 0x150
        //public const int RifleClipAmmo = 0x148;           // rifle 0x128

        public const int xPosOffset = 0x38;
        public const int yPosOffset = 0x3C;
        public const int zPosOffset = 0x34;

        // position: 4 8 C = 34 38 3C

        public const int Yaw = 0x40;    //[0..360]
        public const int Pitch = 0x44;  //[-90..90]
        public const int Roll = 0x48;   //signed
    }
}
