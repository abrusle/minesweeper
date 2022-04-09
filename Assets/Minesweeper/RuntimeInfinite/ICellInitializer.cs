using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    public interface ICellInitializer
    {
        void UpdateCellView(Transform instance, Vector2Int cellPosition, CellStatusFlags cellStatus);
    }
}