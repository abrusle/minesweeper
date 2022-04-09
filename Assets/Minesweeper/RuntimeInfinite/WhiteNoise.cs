using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    public static class WhiteNoise
    {
        public static float Sample(float x, float y, float seed)
        {
            return Hash13(new Vector3(x, y, seed));
        }
        
        //  1 out, 3 in...
        private static float Hash13(Vector3 p3)
        {
            const float a = 0.1031f;
            p3.x = Fract(p3.x * a);
            p3.y = Fract(p3.y * a);
            p3.z = Fract(p3.z * a);

            p3 = p3.Add(Vector3.Dot(p3, p3.Add(31.32f)));

            return Fract((p3.x + p3.y) * p3.z);
        }

        private static float Fract(float x)
        {
            return x - Mathf.Floor(x);
        }
    }
}