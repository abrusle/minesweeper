using System;
using System.Collections.Generic;
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
                var neighbors = new Vector2Int[8];
                LevelUtility.GetAdjacentCellsSquare(new Vector2Int(x, y), neighbors);
                foreach (Vector2Int nPos in neighbors)
                {
                    StartRevealChain(level, nPos.x, nPos.y, onSouldReveal);
                }
            }
        }

        public static void RevealCellsRecursively(Cell[,] level, int x, int y, Dictionary<Vector2Int, Cell> revealList)
        {
            if (revealList.Count > level.Length)
                throw new OverflowException();
            
            var pos = new Vector2Int(x, y);
            if (revealList.ContainsKey(pos)) 
                return;
            
            if (!level.TryGetValue(x, y, out Cell cell))
                return; // Indexes out of range

            if (cell.isRevealed || cell.hasFlag)
                return; // cell is no subject revelation.

            level[x, y].isRevealed = true;
            revealList[pos] = level[x, y];
            
            if (cell.value == 0)
            {
                var neighbors = new Vector2Int[8];
                LevelUtility.GetAdjacentCellsSquare(pos, neighbors);
                foreach (Vector2Int nPos in neighbors)
                {
                    RevealCellsRecursively(level, nPos.x, nPos.y, revealList);
                }
            }
        }
    }
}