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

        public void RevealCell(int x, int y)
        {
            _cellViews[x, y].Reveal();
        }

        public void DrawLevel(Cell[,] level)
        {
            foreach (Transform cell in _grid.transform)
            {
                Destroy(cell.gameObject);
            }

            var root = new GameObject("Level").transform;
            root.SetParent(_grid.transform);
            root.localPosition = Vector3.zero;
            
            var log = new StringBuilder("Generated Level:\n");

            int xMax = level.GetLength(0);
            int yMax = level.GetLength(1);

            _cellViews = new CellView[xMax, yMax];
            
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    var cell = level[x, y];
                    string cellText = cell.hasMine ? "M" : cell.value.ToString();

                    var cellView = Instantiate(cellViewPrefab, root);
                    cellView.name = $"Cell [{x};{y}]";
                    cellView.transform.localPosition = _grid.GetCellCenterLocal(new Vector3Int(x, y, 0));
                    cellView.Load(x, y, cellText);
                    if (!cell.hasMine)
                        cellView.TextColor = colorSheet.GetColor(cell.value);
                    _cellViews[x, y] = cellView;

                    log.Append(cellText).Append(", ");
                }

                log.Append('\n');
            }
            
            MoveLevelCenterToWorldOrigin(xMax, yMax);
            
            Debug.Log(log.ToString());
        }

        private void MoveLevelCenterToWorldOrigin(int xMax, int yMax)
        {
            var gridPos = new Vector3(-xMax, -yMax, 0);
            _grid.transform.position = (_grid.cellSize + _grid.cellGap).MultiplyComponents(gridPos) * 0.5f;;
        }
    }
}