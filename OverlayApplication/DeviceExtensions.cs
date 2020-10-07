using System.Drawing;
using System.Linq;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace OverlayApplication
{
    public static class DeviceExtensions
    {
        public static void DrawPolyline(this Device device, Vector3[] vertices, Color color)
        {
            if (vertices.Length < 2 || vertices.Any(v => !v.IsValidScreen()))
            {
                return;
            }

            var vertexStreamZeroData = vertices.Select(v => new CustomVertex.TransformedColored(v.X, v.Y, v.Z, 0, color.ToArgb())).ToArray();
            device.VertexFormat = VertexFormats.Diffuse | VertexFormats.Transformed;
            device.DrawUserPrimitives(PrimitiveType.LineStrip, vertexStreamZeroData.Length - 1, vertexStreamZeroData);
        }

        public static bool IsInfinityOrNaN(this float value)
        {
            return float.IsNaN(value) || float.IsInfinity(value);
        }

        public static bool IsValidScreen(this Vector3 value)
        {
            return !value.X.IsInfinityOrNaN() && !value.Y.IsInfinityOrNaN() && value.Z >= 0 && value.Z < 1;
        }
    }
}
