using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minesweeper.Runtime
{
    using Data;
    using Views;
    
    public class GameSceneContext : SceneContext
    {
        public GameCameraClassic gameCamera;
        public LevelSettings levelSettings;
        public LevelGridView levelGridView;
        public GameUiView uiView;
        [FormerlySerializedAs("hoverIndicator")] public CursorView cursor;
        
        private Cell[,] _level;
        private int _emptyCellsLeft;
        private GameState _gameState;
        private Vector2Int _cursorGridPos;
        private int _flagCount;

        private enum GameState
        {
            Running, Over, Won
        }

        public void Start()
        {
            _level = null;
            _emptyCellsLeft = levelSettings.size.x * levelSettings.size.y;
            _flagCount = 0;
            _gameState = GameState.Running;
            cursor.positionMin = Vector2Int.zero;
            cursor.positionMax = levelSettings.size - Vector2Int.one;
            levelGridView.OnGameStart(gameCamera);
            gameCamera.FitToLevel(levelSettings.size, levelGridView.Grid);
            uiView.OnMinesLeftCountChange(levelSettings.mineCount);
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

        private void Update()
        {
            if (_gameState == GameState.Running)
            {
                var worldPoint = gameCamera.Camera.ScreenToWorldPoint(Input.mousePosition);
                _cursorGridPos = (Vector2Int) levelGridView.Grid.WorldToCell(worldPoint);
                cursor.UpdatePosition(_cursorGridPos);
            }
        }

        private void OnLeftClick()
        {
            if (_gameState != GameState.Running) return;
            var cellPos = _cursorGridPos;

            if (_level == null || _level.Length == 0)
            {
                if (cellPos.x >= 0 && cellPos.x < levelSettings.size.x &&
                    cellPos.y >= 0 && cellPos.y < levelSettings.size.y) // cell in range
                {
                    var reservedEmpty = new Vector2Int[9];
                    LevelUtility.GetAdjacentCellsSquare(cellPos, reservedEmpty);
                    reservedEmpty[8] = cellPos;
                    
                    _level = LevelGenerator.GenerateNewLevel(
                        levelSettings.size.x,
                        levelSettings.size.y,
                        levelSettings.mineCount,
                        reservedEmpty);
                }
                else return; // no level generated and cell clicked is not in range.
            }

            if (_level.TryGetValue(cellPos, out var cell))
            {
                if (cell.hasFlag)
                    return;
                
                if (cell.hasMine)
                {
                    OnGameOver();
                    return;
                }
            }

            var toReveal = new Dictionary<Vector2Int, Cell>();
            RevealHandler.RevealCellsRecursively(_level, cellPos.x, cellPos.y, toReveal);

            foreach (var pair in toReveal.OrderBy(a => Vector2Int.Distance(cellPos, a.Key)))
            {
                levelGridView.RevealCell(pair.Value, pair.Key.x, pair.Key.y);
            }

            _emptyCellsLeft -= toReveal.Count;
            if (_emptyCellsLeft <= levelSettings.mineCount)
                OnGameWon();
        }

        private void OnRightClick()
        {
            if (_gameState != GameState.Running) return;
            if (_level == null || _level.Length == 0) return;
            var cellPos = _cursorGridPos;

            if (_level.TryGetValue(cellPos, out var cell) && !cell.isRevealed)
            {
                if (cell.hasFlag)
                {
                    levelGridView.UnflagCell(cellPos.x, cellPos.y);
                    _flagCount--;
                }
                else
                {
                    levelGridView.FlagCell(cellPos.x, cellPos.y);
                    _flagCount++;
                }
                
                _level[cellPos.x, cellPos.y].hasFlag = !cell.hasFlag;
                uiView.OnMinesLeftCountChange(levelSettings.mineCount - _flagCount);
            }
        }

        private void OnGameWon()
        {
            _gameState = GameState.Won;
            levelGridView.DrawGameWon(gameCamera);

            for (int x = 0; x < _level.GetLength(0); x++)
            {
                for (int y = 0; y < _level.GetLength(1); y++)
                {
                    var cell = _level[x, y];
                    if (cell.hasMine)
                        levelGridView.FlagCell(x, y);
                }
            }
        }

        private void OnGameOver()
        {
            _gameState = GameState.Over;
            levelGridView.DrawGameOver(gameCamera);
            
            for (int x = 0; x < _level.GetLength(0); x++)
            {
                for (int y = 0; y < _level.GetLength(1); y++)
                {
                    var cell = _level[x, y];
                    if (cell.hasMine) 
                        levelGridView.RevealCell(cell, x, y, false);
                }
            }
        }
    }
}