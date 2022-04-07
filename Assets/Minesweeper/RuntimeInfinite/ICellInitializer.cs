using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    public interface ICellInitializer
    {
        void InitializeCell(Vector2Int cellPosition, Transform instance);
    }
}