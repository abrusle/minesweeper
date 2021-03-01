using UnityEngine;

namespace Minesweeper.Runtime
{
    public static class LevelGenerator
    {
        public static int[,] GenerateNewLevel(int rowCount, int colCount, int mineCount)
        {
            var cells = new int[rowCount, colCount];

            for (int i = 0; i < mineCount; i++)
            {
                var pos = RandomPoint(0, rowCount - 1, 0, colCount - 1);

                // place mine
                cells[pos.x, pos.y] = int.MinValue;

                // indicate neighbors
                if (pos.x < rowCount - 1)
                {
                    cells[pos.x + 1, pos.y]++;

                    if (pos.y < colCount - 1)
                        cells[pos.x + 1, pos.y + 1]++;
                    if (pos.y > 0)
                        cells[pos.x + 1, pos.y - 1]++;
                }

                if (pos.y < colCount - 1)
                    cells[pos.x, pos.y + 1]++;

                if (pos.x > 0)
                {
                    cells[pos.x - 1, pos.y]++;

                    if (pos.y > 0)
                        cells[pos.x - 1, pos.y - 1]++;
                    if (pos.y < colCount - 1)
                        cells[pos.x - 1, pos.y + 1]++;
                }

                if (pos.y > 0)
                    cells[pos.x, pos.y - 1]++;
                
                /*  TODO Fix double count
                0, 0, 3, M, 3, 0, 0, 0, 
                0, 0, 3, M, 3, 0, 0, 0, 
                0, 0, 1, 1, 1, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 
                1, 1, 0, 0, 0, 0, 0, 0, 
                M, 1, 0, 0, 0, 0, 0, 0, 
                1, 1, 0, 0, 0, 0, 0, 0, */
            }

            return cells;
        }

        private static Vector2Int RandomPoint(int minX, int maxX, int minY, int maxY)
        {
            return new Vector2Int
            {
                x = Random.Range(minX, maxX),
                y = Random.Range(minY, maxY)
            };
        }
    }
}