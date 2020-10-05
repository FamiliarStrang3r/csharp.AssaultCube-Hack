
namespace AssaultCubeHack
{
    static class Offsets
    {
        //0x0050F4F8 array pointer
        // + 0x4 bot offset
        // + 0xF8 bot health offset

        public const int EntityList = 0x0050F4F8;

        public const int Health = 0xF8;

        public const int xPosOffset = 0x38;
        public const int yPosOffset = 0x3C;
        public const int zPosOffset = 0x34;

        public const int Yaw = 0x40;      //[0..360]
        public const int Pitch = 0x44;    //[-90..90]
    }
}
