﻿using UnityEngine;

namespace Minesweeper.Runtime.Views
{
    using Data;
    using Animation;
    
    [RequireComponent(typeof(Grid))]
    public class LevelGridView : MonoBehaviour
    {
        public Grid Grid { get; private set; }

        [SerializeField] private CellView cellViewPrefab;
        [SerializeField] private ColorSheet colorSheet;
        [SerializeField, Min(0)] private int revealAnimationInterval;
        

        private CellView[,] _cellViews;
        private AnimationSequence<RevealAnimationDatum> _revealSequence;

        private void Awake()
        {
            Grid = GetComponent<Grid>();
            _revealSequence = new AnimationSequence<RevealAnimationDatum>(PlayRevealAnimation)
            {
                IntervalMiliseconds = revealAnimationInterval
            };
        }

        private void OnDestroy()
        {
            _revealSequence.Dispose();
        }

        public void RevealCell(Cell cell, int x, int y, bool animated = true)
        {
            var datum = new RevealAnimationDatum(cell, x, y);
            if (animated)
                _revealSequence.AddAnimation(datum);
            else
                PlayRevealAnimation(datum);
        }

        public void FlagCell(int x, int y)
        {
            var cellView = _cellViews[x,y];
            cellView.backgroundSprite.color = colorSheet.flaggedCellColor; //new Color(1f, 0.94f, 0.51f);
            cellView.FlagColor = colorSheet.flagColor; //new Color(0.43f, 0.36f, 0.15f);
            cellView.ToggleFlag(true);
        }

        public void UnflagCell(int x, int y)
        {
            var cellView = _cellViews[x,y];
            cellView.backgroundSprite.color = colorSheet.unrevealedCellColor;
            cellView.FlagColor = colorSheet.GetColor(0);
            cellView.ToggleFlag(false);
        }

        public void Clear()
        {
            _revealSequence.Dispose();
            foreach (Transform level in Grid.transform)
            {
                Destroy(level.gameObject);
            }

            _cellViews = null;
        }

        public void DrawLevelGrid(int xMax, int yMax)
        {
            Clear();

            var root = new GameObject($"Level ({xMax} x {yMax})").transform;
            root.SetParent(Grid.transform);
            root.localPosition = Vector3.zero;
            
            _cellViews = new CellView[xMax, yMax];
            
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    var cellView = Instantiate(cellViewPrefab, root);
                    cellView.name = $"Cell [{x};{y}]";
                    cellView.transform.localPosition = Grid.GetCellCenterLocal(new Vector3Int(x, y, 0));
                    _cellViews[x, y] = cellView;
                    cellView.textMesh.text = string.Empty;
                    cellView.textMesh.enabled = false;
                    cellView.backgroundSprite.color = colorSheet.unrevealedCellColor;
                }
            }
            
            MoveLevelCenterToWorldOrigin(xMax, yMax);
        }

        public void OnGameStart(Camera bgCamera)
        {
            bgCamera.backgroundColor = colorSheet.bgColor;
        }

        public void DrawGameOver(Camera bgCamera)
        {
            bgCamera.backgroundColor = colorSheet.gameOverBgColor;
        }

        public void DrawGameWon(Camera bgCamera)
        {
            bgCamera.backgroundColor = colorSheet.gameWonBgColor;
        }

        private void MoveLevelCenterToWorldOrigin(int xMax, int yMax)
        {
            var gridPos = new Vector3(-xMax, -yMax, 0);
            Grid.transform.position = (Grid.cellSize + Grid.cellGap).MultiplyComponents(gridPos) * 0.5f;;
        }

        private void PlayRevealAnimation(RevealAnimationDatum datum)
        {
            var cellView = _cellViews[datum.x, datum.y];
            if (datum.cell.value != 0)
            {
                cellView.textMesh.text = datum.cell.hasMine ? "*" : datum.cell.value.ToString();
                if (!datum.cell.hasMine)
                    cellView.textMesh.color = colorSheet.GetColor(datum.cell.value);
                else
                {
                    cellView.textMesh.color = colorSheet.mineColor;
                    cellView.textMesh.fontSize = 8;
                }
                
                cellView.textMesh.enabled = true;
            }

            cellView.backgroundSprite.color = colorSheet.revealedCellColor;
        }

        private readonly struct RevealAnimationDatum
        {
            public readonly Cell cell;
            public readonly int x, y;

            public RevealAnimationDatum(Cell cell, int x, int y)
            {
                this.cell = cell;
                this.x = x;
                this.y = y;
            }
        }
    }
}