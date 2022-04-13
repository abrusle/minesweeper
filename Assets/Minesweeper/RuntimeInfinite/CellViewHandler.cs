using Minesweeper.Runtime.Data;
using Minesweeper.Runtime.Utility;
using Minesweeper.Runtime.Views;
using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    public class CellViewHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InfiniteLevelDataManager levelDataManager;
        
        [Header("Settings")]
        [SerializeField] private ColorSheet colorSheet;

        public void SetCellBackground(CellView view, bool isRevealed, bool isMarked)
        {
            view.backgroundSprite.color = isRevealed
                ? colorSheet.revealedCellColor
                : isMarked
                    ? colorSheet.flaggedCellColor
                    : colorSheet.unrevealedCellColor;
        }

        private void SetMineMode(CellView view, bool isMineMode)
        {
            view.textMesh.enabled = !isMineMode;
            view.mineSprite.enabled = isMineMode;
        }

        public void UpdateInstance(CellView view, Vector2Int coords, CellStatusFlags cellStatus)
        {
            if (view == null) return;

            bool isRevealed = cellStatus.HasFlag(CellStatusFlags.IsRevealed);
            bool isMarked = !isRevealed && cellStatus.HasFlag(CellStatusFlags.IsMarked);
            view.textMesh.enabled = isRevealed;
            view.ToggleFlag(isMarked);
            SetCellBackground(view, isRevealed, isMarked);
            if (isMarked)
            {
                view.FlagColor = colorSheet.flagColor;
            }
            
            if (isRevealed)
            {
                if (cellStatus.HasFlag(CellStatusFlags.HasMine))
                {
                    SetMineMode(view, true);
                }
                else
                {
                    SetMineMode(view, false);
                    const int neighborCount = 8;
                    var neighbors = ArrayPool<Vector2Int>.Get(neighborCount);
                    LevelUtility.GetAdjacentCellsSquare(coords, neighbors);
                    int cellValue = 0;
                    for (int i = 0; i < neighborCount; ++i)
                    {
                        var neighborStatus = levelDataManager.GetCellStatus(neighbors[i]);
                        if (neighborStatus.HasFlag(CellStatusFlags.HasMine))
                        {
                            cellValue++;
                        }
                    }

                    ArrayPool<Vector2Int>.Release(neighbors);

                    view.textMesh.text = cellValue == 0 ? "" : cellValue.ToString();
                    view.textMesh.color = colorSheet.GetColorForCellValue(cellValue);
                }
            }
            else
            {
                view.mineSprite.enabled = false;
            }
        }
        
    }
}