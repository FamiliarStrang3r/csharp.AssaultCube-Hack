using System;

namespace AssaultCubeHack
{
    class Entity
    {
        public int baseAddress;

        public Vector3 position;
        public int health;

        private VAMemory vam;
        private int pointer;

        public Entity(int pointer)
        {
            this.pointer = pointer;
        }

        public void Init(VAMemory vam)
        {
            this.vam = vam;
            baseAddress = vam.ReadInt32((IntPtr)pointer);
        }

        public void Calculate()
        {
            position.x = vam.ReadFloat((IntPtr)(baseAddress + Offsets.xPosOffset));
            position.y = vam.ReadFloat((IntPtr)(baseAddress + Offsets.yPosOffset));
            position.z = vam.ReadFloat((IntPtr)(baseAddress + Offsets.zPosOffset));

            health = vam.ReadInt32((IntPtr)(baseAddress + Offsets.Health));
        }

        public void SetRotation(float pitch, float yaw)
        {
            vam.WriteFloat((IntPtr)baseAddress + Offsets.Pitch, pitch);
            vam.WriteFloat((IntPtr)baseAddress + Offsets.Yaw, yaw);
        }

        public void SetHealth(int amount)
        {
            vam.WriteInt32((IntPtr)baseAddress + Offsets.Health, amount);
        }
    }
}
