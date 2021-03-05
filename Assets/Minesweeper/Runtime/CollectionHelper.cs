using UnityEngine;

namespace Minesweeper.Runtime
{
    public static class CollectionHelper
    {
        public static bool TryGetValue<T>(this T[,] arr, Vector2Int i, out T value)
        {
            return TryGetValue(arr, i.x, i.y, out value);
        }

        public static bool TryGetValue<T>(this T[,] arr, int i0, int i1, out T value)
        {
            if (arr.HasValuesIsInRange(i0, i1))
            {
                value = arr[i0, i1];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public static bool HasValuesIsInRange<T>(this T[,] arr, Vector2Int i)
        {
            return arr.HasValuesIsInRange(i.x, i.y);
        }
        public static bool HasValuesIsInRange<T>(this T[,] arr, int i0, int i1)
        {
            return !(i0 < 0 || i0 > arr.GetLength(0) - 1 ||
                     i1 < 0 || i1 > arr.GetLength(1) - 1);
        }
    }
}