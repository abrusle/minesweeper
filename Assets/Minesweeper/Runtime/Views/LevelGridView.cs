﻿using System.Text;
using UnityEngine;
namespace Minesweeper.Runtime.Views
{
    using Data;
    
    [RequireComponent(typeof(Grid))]
    public class LevelGridView : MonoBehaviour
    {
        public Grid Grid => _grid;

        [SerializeField]
        private CellView cellViewPrefab;

        [SerializeField] private CellColorSheet colorSheet;

        private Grid _grid;
        private CellView[,] _cellViews;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
        }

        public void RevealCell(Cell cell, int x, int y)
        {
            var cellView = _cellViews[x, y];
            if (cell.value != 0)
            {
                cellView.textMesh.text = cell.hasMine ? "M" : cell.value.ToString();
                if (!cell.hasMine)
                    cellView.textMesh.color = colorSheet.GetColor(cell.value);
                cellView.textMesh.enabled = true;
            }
            cellView.backgroundSprite.color = colorSheet.revealedColor;
        }

        public void FlagCell(int x, int y)
        {
            _cellViews[x,y].backgroundSprite.color = new Color(1f, 0.94f, 0.51f);
        }

        public void UnflagCell(int x, int y)
        {
            _cellViews[x,y].backgroundSprite.color = colorSheet.unrevealedColor;
        }

        public void Clear()
        {
            foreach (Transform level in _grid.transform)
            {
                Destroy(level.gameObject);
            }

            _cellViews = null;
        }

        public void DrawLevelGrid(int xMax, int yMax)
        {
            Clear();

            var root = new GameObject($"Level ({xMax} x {yMax})").transform;
            root.SetParent(_grid.transform);
            root.localPosition = Vector3.zero;
            
            _cellViews = new CellView[xMax, yMax];
            
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    var cellView = Instantiate(cellViewPrefab, root);
                    cellView.name = $"Cell [{x};{y}]";
                    cellView.transform.localPosition = _grid.GetCellCenterLocal(new Vector3Int(x, y, 0));
                    _cellViews[x, y] = cellView;
                    cellView.textMesh.text = string.Empty;
                    cellView.textMesh.enabled = false;
                    cellView.backgroundSprite.color = colorSheet.unrevealedColor;
                }
            }
            
            MoveLevelCenterToWorldOrigin(xMax, yMax);
        }

        private void MoveLevelCenterToWorldOrigin(int xMax, int yMax)
        {
            var gridPos = new Vector3(-xMax, -yMax, 0);
            _grid.transform.position = (_grid.cellSize + _grid.cellGap).MultiplyComponents(gridPos) * 0.5f;;
        }
    }
}