using System;
using Minesweeper.Runtime.Views;
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
                return; // failed to reveal the cell

            if (cell.value == 0)
            {
                var neighbors = new Vector2Int[8];
                LevelUtility.GetSquareNeighbors(new Vector2Int(x, y), neighbors);
                foreach (Vector2Int nPos in neighbors)
                {
                    StartRevealChain(level, nPos.x, nPos.y, onSouldReveal);
                }
            }
        }
    }
}