using System;

namespace Minesweeper.Runtime
{
    public static class CollectionHelper
    {
        public static bool TryGetValue<T>(this T[,] arr, int i0, int i1, out T value)
        {
            if (i0 < 0 || i1 < 0 
                || i0 > arr.GetLength(0) - 1
                || i1 > arr.GetLength(1) - 1)
            {
                value = default;
                return false;
            }
            
            value = arr[i0, i1];
            return true;
            
        }
    }
}