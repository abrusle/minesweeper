using System;
using Minesweeper.Runtime.Views;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minesweeper.Runtime
{
    public static class LevelGenerator
    {
        public static Cell[,] GenerateNewLevel(int xMax, int yMax, int mineCount, params Vector2Int[] reservedEmptyCells)
        {
            var cells = new Cell[xMax, yMax];
            var mines = GenerateRandomPoints(mineCount, 0, xMax - 1, 0, yMax - 1, reservedEmptyCells ?? new Vector2Int[0]);
            
            foreach (Vector2Int pos in mines)
            {
                // place mine
                cells[pos.x, pos.y].value = int.MinValue;
                cells[pos.x, pos.y].hasMine = true;

                var mineNeighbors = new Vector2Int[8];
                LevelUtility.GetSquareNeighbors(pos, mineNeighbors);
                foreach (var neighbor in mineNeighbors)
                {
                    if (cells.HasValuesIsInRange(neighbor))
                        cells[neighbor.x, neighbor.y].value++;
                }

                // indicate neighbors
                /*if (pos.x < xMax - 1)
                {
                    cells[pos.x + 1, pos.y].value++;

                    if (pos.y < yMax - 1)
                        cells[pos.x + 1, pos.y + 1].value++;
                    if (pos.y > 0)
                        cells[pos.x + 1, pos.y - 1].value++;
                }

                if (pos.y < yMax - 1)
                    cells[pos.x, pos.y + 1].value++;

                if (pos.x > 0)
                {
                    cells[pos.x - 1, pos.y].value++;

                    if (pos.y > 0)
                        cells[pos.x - 1, pos.y - 1].value++;
                    if (pos.y < yMax - 1)
                        cells[pos.x - 1, pos.y + 1].value++;
                }

                if (pos.y > 0)
                    cells[pos.x, pos.y - 1].value++;*/
            }

            return cells;
        }

        private static Vector2Int[] GenerateRandomPoints(int count, int xMin, int xMax, int yMin, int yMax, Vector2Int[] exculdedPoints)
        {
            if (count > (xMax - xMin) * (yMax - yMin))
                throw new InvalidRangeException();
            
            var points = new Vector2Int[count];
            for (int i = 0; i < count; i++)
            {
                var pt = RandomPoint(xMin, xMax, yMin, yMax);
                if (Array.IndexOf(points, pt) != -1 || // a point with these coordinates was already generated
                    Array.IndexOf(exculdedPoints, pt) != -1) // pt is part of the points to exculde
                    i--; // redo the loop for the same value of i;
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