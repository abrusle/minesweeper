using System;
using UnityEngine;

namespace Minesweeper.Runtime
{
    public static class LevelUtility
    {
        public static void GetSquareNeighbors(Vector2Int cell, Vector2Int[] neighbors)
        {
            if (neighbors.Length < 8)
                throw new ArgumentException("neighbors array must be of length 8 or more");
            
            GetImmediateNeighbors(cell, neighbors);
            neighbors[4] = new Vector2Int(cell.x + 1, cell.y + 1);
            neighbors[5] = new Vector2Int(cell.x + 1, cell.y - 1);
            neighbors[6] = new Vector2Int(cell.x - 1, cell.y + 1);
            neighbors[7] = new Vector2Int(cell.x - 1, cell.y - 1);
        }

        public static void GetImmediateNeighbors(Vector2Int cell, Vector2Int[] neighbors)
        {
            if (neighbors.Length < 4)
                throw new ArgumentException("neighbors array must be of length 4 or more");
            
            neighbors[0] = new Vector2Int(cell.x + 1, cell.y);
            neighbors[1] = new Vector2Int(cell.x, cell.y + 1);
            neighbors[2] = new Vector2Int(cell.x, cell.y - 1);
            neighbors[3] = new Vector2Int(cell.x - 1, cell.y);
        }
    }
}