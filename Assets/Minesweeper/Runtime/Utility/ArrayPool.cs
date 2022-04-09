using System.Collections.Generic;

namespace Minesweeper.Runtime.Utility
{
    public static class ArrayPool<TArray>
    {
        private static readonly Dictionary<int, Stack<TArray[]>> _PoolStacks = new();

        public static TArray[] Get(int arraySize)
        {
            var stack = GetStack(arraySize);

            if (stack.Count > 0)
            {
                return stack.Pop();
            }

            return new TArray[arraySize];
        }

        public static void Release(TArray[] array)
        {
            int arraySize = array.Length;
            GetStack(arraySize).Push(array);
        }

        private static Stack<TArray[]> GetStack(int arraySize)
        {
            if (!_PoolStacks.TryGetValue(arraySize, out var stack))
            {
                stack = new Stack<TArray[]>();
                _PoolStacks[arraySize] = stack;
            }

            return stack;
        }
    }
}