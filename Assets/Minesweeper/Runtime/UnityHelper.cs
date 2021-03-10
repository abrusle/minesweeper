using UnityEngine;

namespace Minesweeper.Runtime
{
    public static class UnityHelper
    {
        public static Vector3 MultiplyComponents(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3Int AddZ(this Vector2Int v2, int z)
        {
            return new Vector3Int(v2.x, v2.y, z);
        }
    }
}