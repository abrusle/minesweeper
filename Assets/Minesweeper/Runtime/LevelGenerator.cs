using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minesweeper.Runtime
{
    public static class LevelGenerator
    {
        public static int[,] GenerateNewLevel(int rowCount, int colCount, int mineCount)
        {
            var cells = new int[rowCount, colCount];
            var mines = GenerateRandomPoints(mineCount, 0, rowCount - 1, 0, colCount - 1);
            
            foreach (Vector2Int pos in mines)
            {
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
            }

            return cells;
        }

        private static Vector2Int[] GenerateRandomPoints(int count, int xMin, int xMax, int yMin, int yMax)
        {
            if (xMax - xMin < count || yMax - yMin < count)
                throw new InvalidRangeException();
            
            var points = new Vector2Int[count];
            for (int i = 0; i < count; i++)
            {
                var pt = RandomPoint(xMin, xMax, yMin, yMax);
                if (Array.IndexOf(points, pt) != -1) i--;
                else points[i] = pt;
            }

            return points;
        }

        private static Vector2Int RandomPoint(int xMin, int xMax, int yMin, int yMax)
        {
            return new Vector2Int
            {
                x = Random.Range(xMin, xMax),
                y = Random.Range(yMin, yMax)
            };
        }
        
        private class InvalidRangeException : System.Exception
        {
            
        }
    }
}