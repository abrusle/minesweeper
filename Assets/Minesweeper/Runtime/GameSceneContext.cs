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

        private void Awake()
        {
            GenerateLevel();
        }
        
        private void OnEnable()
        {
            InputHandler.LeftClick += OnLeftClick;
        }

        private void OnDisable()
        {
            InputHandler.LeftClick -= OnLeftClick;
        }

        private void OnLeftClick()
        {
            var worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var cellPos = (Vector2Int) levelGridView.Grid.WorldToCell(worldPoint);
            
            // TODO Generate level on first click. Ensure that cellPos contains a 0.

            RevealHandler.StartRevealChain(_level, cellPos.x, cellPos.y, (cell, pos) =>
            {
                levelGridView.RevealCell(pos.x, pos.y);
                return true;
            });
        }

        private void GenerateLevel()
        {
            _level = LevelGenerator.GenerateNewLevel(
                levelSettings.rowCount,
                levelSettings.columnCount,
                levelSettings.mineCount);
            levelGridView.DrawLevel(_level);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Regenerate"))
            {
                GenerateLevel();
            }
        }
    }
}