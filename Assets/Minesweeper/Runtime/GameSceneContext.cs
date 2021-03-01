using System;
using Minesweeper.Runtime.Data;
using Minesweeper.Runtime.Views;
using UnityEngine;

namespace Minesweeper.Runtime
{
    public class GameSceneContext : SceneContext
    {
        public LevelSettings levelSettings;
        public LevelGridView levelGridView;
        
        private void Awake()
        {
            var level = LevelGenerator.GenerateNewLevel(levelSettings.rowCount, levelSettings.columnCount, levelSettings.mineCount);
            levelGridView.DrawLevel(level);
        }
    }
}