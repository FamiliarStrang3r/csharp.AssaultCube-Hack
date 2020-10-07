using System;
using Microsoft.DirectX;

namespace OverlayApplication
{
    public class Entity
    {
        public Vector3 HeadPosition;
        public Vector3 FootPosition;

        public int Team;
        public string Name;
        public int Health;
        public int Armor;

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
        }

        public override string ToString()
        {
            return $"Team: {Team}, Health: {Health}, HeadPosition: {HeadPosition.ToString()}";
        }
    }
}
