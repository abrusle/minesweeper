using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minesweeper.Runtime
{
    using Data;
    using Views;
    
    /// <summary>
    /// To Refactor
    /// Split into :
    ///  - SceneContext
    ///  - Level Manager
    ///  - LevelData (cell table)
    /// </summary>
    public class GameSceneContext : SceneContext
    {
        public GameCameraClassic gameCamera;
        public LevelSettings levelSettings;
        public LevelGridView levelGridView;
        public GameUiView uiView;
        public LevelInputEvents inputEvents;
        [FormerlySerializedAs("hoverIndicator")] public CursorView cursor;
        
        private LevelTable _level = new LevelTable();
        private int _emptyCellsLeft;
        private LevelState _levelState;
        private Vector2Int _cursorGridPos;
        private int _flagCount;

        private enum LevelState
        {
            Running, Over, Won
        }

        public void Start()
        {
            _level.Clear();
            _emptyCellsLeft = levelSettings.size.x * levelSettings.size.y;
            _flagCount = 0;
            _levelState = LevelState.Running;
            levelGridView.OnGameStart(gameCamera);
            gameCamera.FitToLevel(levelSettings.size, levelGridView.Grid);
            uiView.OnMinesLeftCountChange(levelSettings.mineCount);
            levelGridView.DrawLevelGrid(levelSettings.size.x, levelSettings.size.y);
        }
        
        private void OnEnable()
        {
            inputEvents.NormalClick.OnClicked.AddListener(OnLeftClick);
            inputEvents.SpecialClick.OnClicked.AddListener(OnRightClick);
        }

        private void OnDisable()
        {
            inputEvents.NormalClick.OnClicked.RemoveListener(OnLeftClick);
            inputEvents.SpecialClick.OnClicked.RemoveListener(OnRightClick);
        }

        private void OnLeftClick(Vector2Int cellPos)
        {
            if (_levelState != LevelState.Running)
                return;

            if (_level.Generated == false)
            {
                var reservedEmpty = new Vector2Int[9];
                LevelUtility.GetAdjacentCellsSquare(cellPos, reservedEmpty);
                reservedEmpty[8] = cellPos;
                _level.Generate(levelSettings, reservedEmpty);
            }

            Cell cell = _level[cellPos];

            if (cell.hasFlag)
                return;
            
            if (cell.hasMine)
            {
                OnGameOver();
                return;
            }

            var toReveal = new Dictionary<Vector2Int, Cell>();
            RevealHandler.RevealCellsRecursively(_level, cellPos.x, cellPos.y, toReveal);

            foreach (var pair in toReveal.OrderBy(a => Vector2Int.Distance(cellPos, a.Key)))
            {
                levelGridView.RevealCell(pair.Value, pair.Key.x, pair.Key.y);
            }

            _emptyCellsLeft -= toReveal.Count;
            if (_emptyCellsLeft <= _level.MineCount)
                OnGameWon();
        }

        private void OnRightClick(Vector2Int cellPos)
        {
            if (_levelState != LevelState.Running) return;
            if (_level.Generated == false) return;

            Cell cell = _level[cellPos];
            if (!cell.isRevealed)
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

                _level.ToggleCellFlag(cellPos);
                uiView.OnMinesLeftCountChange(levelSettings.mineCount - _flagCount);
            }
        }

        private void OnGameWon()
        {
            _levelState = LevelState.Won;
            levelGridView.DrawGameWon(gameCamera);

            foreach (var tableCell in _level)
            {
                if (tableCell.data.hasMine)
                        levelGridView.FlagCell(tableCell.position.x, tableCell.position.y);
            }
        }

        private void OnGameOver()
        {
            _levelState = LevelState.Over;
            levelGridView.DrawGameOver(gameCamera);
            
            foreach (var tableCell in _level)
            {
                if (tableCell.data.hasMine)
                    levelGridView.RevealCell(tableCell.data,
                        tableCell.position.x,
                        tableCell.position.y,
                        false);
            }
        }
    }
}