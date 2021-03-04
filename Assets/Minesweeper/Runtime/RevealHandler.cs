using System;
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
                StartRevealChain(level, x + 1, y + 1, onSouldReveal);
                StartRevealChain(level, x + 1, y, onSouldReveal);
                StartRevealChain(level, x + 1, y - 1, onSouldReveal);
                
                StartRevealChain(level, x, y + 1, onSouldReveal);
                StartRevealChain(level, x, y - 1, onSouldReveal);
                
                StartRevealChain(level, x - 1, y + 1, onSouldReveal);
                StartRevealChain(level, x - 1, y, onSouldReveal);
                StartRevealChain(level, x - 1, y - 1, onSouldReveal);
            }
        }
    }
}