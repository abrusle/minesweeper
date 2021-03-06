using Minesweeper.Runtime.Data;
using Minesweeper.Runtime.Views;
using UnityEngine;

namespace Minesweeper.Runtime
{
    public class GameSceneContext : SceneContext
    {
        public Camera mainCamera;
        public LevelSettings levelSettings;
        public LevelGridView levelGridView;
        private Cell[,] _level;

        private void Start()
        {
            _level = null;
            levelGridView.DrawLevelGrid(levelSettings.size.x, levelSettings.size.y);
        }
        
        private void OnEnable()
        {
            InputHandler.LeftClick += OnLeftClick;
            InputHandler.RightClick += OnRightClick;
        }

        private void OnDisable()
        {
            InputHandler.LeftClick -= OnLeftClick;
            InputHandler.RightClick -= OnRightClick;
        }

        private void OnLeftClick()
        {
            var cellPos = GetCellPositionAtCursor();

            if (_level == null || _level.Length == 0)
            {
                if (cellPos.x >= 0 && cellPos.x < levelSettings.size.x &&
                    cellPos.y >= 0 && cellPos.y < levelSettings.size.y) // cell in range
                {
                    var reservedEmpty = new Vector2Int[9];
                    LevelUtility.GetSquareNeighbors(cellPos, reservedEmpty);
                    reservedEmpty[8] = cellPos;
                    
                    _level = LevelGenerator.GenerateNewLevel(
                        levelSettings.size.x,
                        levelSettings.size.y,
                        levelSettings.mineCount,
                        reservedEmpty);
                }
                else return; // no level generated and cell clicked is not in range.
            }
            
            RevealHandler.StartRevealChain(_level, cellPos.x, cellPos.y, (cell, pos) =>
            {
                levelGridView.RevealCell(cell, pos.x, pos.y);
                return true;
            });
        }

        private void OnRightClick()
        {
            if (_level == null || _level.Length == 0) return;
            var cellPos = GetCellPositionAtCursor();

            if (_level.TryGetValue(cellPos, out var cell) && !cell.isRevealed)
            {
                if (cell.hasFlag) levelGridView.UnflagCell(cellPos.x, cellPos.y);
                else levelGridView.FlagCell(cellPos.x, cellPos.y);
                _level[cellPos.x, cellPos.y].hasFlag = !cell.hasFlag;
            }
        }

        private Vector2Int GetCellPositionAtCursor()
        {
            var worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            return (Vector2Int) levelGridView.Grid.WorldToCell(worldPoint);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Restart"))
            {
                Start();
            }
        }
    }
}