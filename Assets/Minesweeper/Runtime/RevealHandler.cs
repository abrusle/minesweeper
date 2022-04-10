using System;
using System.Collections.Generic;
using Minesweeper.Runtime.Utility;
using UnityEngine;

namespace Minesweeper.Runtime
{
    public static class RevealHandler
    {
        public static void StartRevealChain(Cell[,] level, int x, int y, Func<Cell, Vector2Int, bool> onSouldReveal)
        {
            if (!level.TryGetValue(x, y, out Cell cell))
                return; // Indexes out of range

            if (cell.isRevealed || cell.hasFlag)
                return; // cell is no subject revelation.

            if ((level[x, y].isRevealed = onSouldReveal(cell, new Vector2Int(x, y))) == false)
                return; // reveal result asks to stop chain.

            if (cell.value == 0)
            {
                const int neighborCount = 8;
                var neighbors = ArrayPool<Vector2Int>.Get(neighborCount);
                LevelUtility.GetAdjacentCellsSquare(new Vector2Int(x, y), neighbors);
                for (int i = 0; i < neighborCount; i++)
                {
                    Vector2Int nPos = neighbors[i];
                    StartRevealChain(level, nPos.x, nPos.y, onSouldReveal);
                }

                ArrayPool<Vector2Int>.Release(neighbors);
            }
        }

        public static void RevealCellsRecursively(LevelTable level, int x, int y, Dictionary<Vector2Int, Cell> revealList)
        {
            if (revealList.Count > level.CellCount)
                throw new OverflowException();
            
            var pos = new Vector2Int(x, y);
            if (revealList.ContainsKey(pos)) 
                return;

            if (!LevelUtility.IsCellWithinBounds(x, y, level.Size))
                return; // Indexes out of range.

            Cell cell = level[x, y];
            if (cell.isRevealed || cell.hasFlag)
                return; // cell is not subject revelation.

            level.MarkCellRevealed(pos);
            revealList[pos] = level[x, y];
            
            if (cell.value == 0)
            {
                const int neighborCount = 8;
                var neighbors = ArrayPool<Vector2Int>.Get(neighborCount);
                LevelUtility.GetAdjacentCellsSquare(pos, neighbors);
                for (int i = 0; i < neighborCount; i++)
                {
                    Vector2Int nPos = neighbors[i];
                    RevealCellsRecursively(level, nPos.x, nPos.y, revealList);
                }
                
                ArrayPool<Vector2Int>.Release(neighbors);
            }
        }
    }
}