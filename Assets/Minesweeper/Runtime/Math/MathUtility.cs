using UnityEngine;

namespace Minesweeper.Runtime.Math
{
    public static class MathUtility
    {
        public static float SqrDistance(Vector2 a, Vector2 b)
        {
            float num1 = a.x - b.x;
            float num2 = a.y - b.y;
            return (float) (num1 * (double) num1 + num2 * (double) num2);
        }
        
        public static float SqrDistance(Vector3 a, Vector3 b)
        {
            float num1 = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float) (num1 * (double) num1 + num2 * (double) num2 + num3 * (double) num3);
        }
    }
}