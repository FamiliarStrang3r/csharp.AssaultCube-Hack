using System;
using System.Linq;

namespace AssaultCubeHack
{
    class Entity
    {
        public int baseAddress;

        public Vector3 position;
        public int health;
        public string name;
        public int team;

        private VAMemory vam;
        private int pointer;

        //private byte[] bytes;//testing

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
            //name = vam.ReadStringUnicode((IntPtr)(baseAddress + Offsets.Name), 16);
            team = vam.ReadInt32((IntPtr)(baseAddress + Offsets.Team));

            var bytes = vam.ReadByteArray((IntPtr)(baseAddress + Offsets.Name), 16);
            bytes = bytes.Where(b => b > 1).ToArray();
            name = System.Text.Encoding.UTF8.GetString(bytes);
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
