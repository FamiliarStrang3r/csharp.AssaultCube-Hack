using System;
using Microsoft.DirectX;

namespace OverlayApplication
{
    public class Entity
    {
        public Vector3 HeadPosition;
        public Vector3 FootPosition;
        //public Vector3 EulerAngles;

        public int Team;
        public string Name;
        public int Health;
        public int Armor;

        public float Pitch;
        public float Yaw;

        private int Pointer;

        public Entity(int pointer)
        {
            Pointer = pointer;
        }

        public void Update(GameProcess game)
        {
            IntPtr baseAddress = game.Process.Read<IntPtr>((IntPtr)Pointer);

            HeadPosition = game.Process.Read<Vector3>(baseAddress + Offsets.HeadPosition);
            FootPosition = game.Process.Read<Vector3>(baseAddress + Offsets.FootPosition);

            Team = game.Process.Read<int>(baseAddress + Offsets.Team);
            //Name = game.Process.Read<string>(baseAddress + Offsets.Name);
            Health = game.Process.Read<int>(baseAddress + Offsets.Health);
            Armor = game.Process.Read<int>(baseAddress + Offsets.Armor);

            Pitch = game.Process.Read<float>(baseAddress + Offsets.Pitch);
            Yaw = game.Process.Read<float>(baseAddress + Offsets.Yaw);
        }

        public void AimTowards(GameProcess game, Entity entity)
        {
            Vector3 dir = entity.HeadPosition - new Vector3(0, 0, 1) - HeadPosition;
            //float distance = Vector3.Distance(player.position, position);
            var distance = dir.Length();

            float Rad2Deg = 180 / (float)Math.PI;

            float pitch = (float)Math.Asin(dir.Z / distance) * Rad2Deg;
            float yaw = -(float)Math.Atan2(dir.Y, dir.X) * Rad2Deg + 180;
            yaw = (float)Math.Atan2(dir.Y, dir.X) * Rad2Deg + 90;

            /*
            int increments = 10;

            vec3 dst = CalcAngle(mypos, enemypos);
            vec3 diff = dst - src;
            SetAngles(src + diff / increments);
            */

            int increments = 2;

            IntPtr baseAddress = game.Process.Read<IntPtr>((IntPtr)Pointer);

            game.Process.Write(baseAddress + Offsets.Pitch, Pitch + (pitch - Pitch) / increments);
            game.Process.Write(baseAddress + Offsets.Yaw, Yaw + (yaw - Yaw) / increments);
        }

        public void SetPosition(GameProcess game, Vector3 footPosition)
        {
            IntPtr baseAddress = game.Process.Read<IntPtr>((IntPtr)Pointer);

            game.Process.Write(baseAddress + 0x34, footPosition.X);
            game.Process.Write(baseAddress + 0x38, footPosition.Y);
            game.Process.Write(baseAddress + 0x3C, footPosition.Z);
        }

        public override string ToString()
        {
            return $"Team: {Team}, Health: {Health}, HeadPosition: {HeadPosition.ToString()}";
        }
    }
}
