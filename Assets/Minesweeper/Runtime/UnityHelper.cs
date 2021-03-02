using UnityEngine;

namespace Minesweeper.Runtime
{
    public static class UnityHelper
    {
        public static Vector3 MultiplyComponents(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
    }
}