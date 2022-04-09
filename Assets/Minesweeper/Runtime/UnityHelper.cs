using UnityEngine;

namespace Minesweeper.Runtime
{
    public static class UnityHelper
    {
        public static Vector3 Add(this Vector3 a, float f)
        {
            a.x += f;
            a.y += f;
            a.z += f;
            return a;
        }

        public static Vector3Int AddZ(this Vector2Int v2, int z)
        {
            return new Vector3Int(v2.x, v2.y, z);
        }
    }
}