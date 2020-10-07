using System;
using System.Runtime.InteropServices;
using Microsoft.DirectX;

namespace OverlayApplication
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix
    {
        //DirectX: Usualy Row-Major
        //OpenGL: Usualy Column-Major

        //             X,   Y,   Z,   W
        public float m11, m12, m13, m14; //00, 01, 02, 03
        public float m21, m22, m23, m24; //04, 05, 06, 07
        public float m31, m32, m33, m34; //08, 09, 10, 11
        public float m41, m42, m43, m44; //12, 13, 14, 15

        public bool WorldToScreen(Vector3 worldPos, int width, int height, out Vector3 screenPos)
        {
            //multiply vector against matrix
            float screenX = (m11 * worldPos.X) + (m21 * worldPos.Y) + (m31 * worldPos.Z) + m41;
            float screenY = (m12 * worldPos.X) + (m22 * worldPos.Y) + (m32 * worldPos.Z) + m42;
            float screenW = (m14 * worldPos.X) + (m24 * worldPos.Y) + (m34 * worldPos.Z) + m44;

            //camera position (eye level/middle of screen)
            float camX = width / 2f;
            float camY = height / 2f;

            //convert to homogeneous position
            float x = camX + (camX * screenX / screenW);
            float y = camY - (camY * screenY / screenW);
            screenPos = new Vector3(x, y, 0);

            //check if object is behind camera / off screen (not visible)
            //w = z where z is relative to the camera 

            return screenW > 0.001f;
        }

        public override string ToString()
        {
            //display matrix cleanly in a grid
            return string.Format(
                "{0,8}{1,8}{2,8}{3,8}\n" +
                "{4,8}{5,8}{6,8}{7,8}\n" +
                "{8,8}{9,8}{10,8}{11,8}\n" +
                "{12,8}{13,8}{14,8}{15,8}",
                Math.Round(m11, 2), Math.Round(m12, 2), Math.Round(m13, 2), Math.Round(m14, 2),
                Math.Round(m21, 2), Math.Round(m22, 2), Math.Round(m23, 2), Math.Round(m24, 2),
                Math.Round(m31, 2), Math.Round(m32, 2), Math.Round(m33, 2), Math.Round(m34, 2),
                Math.Round(m41, 2), Math.Round(m42, 2), Math.Round(m43, 2), Math.Round(m44, 2));
        }
    }
}
