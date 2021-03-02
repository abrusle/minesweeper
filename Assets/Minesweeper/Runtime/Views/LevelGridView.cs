using System;
using System.Text;
using Minesweeper.Runtime.Data;
using UnityEngine;
using TMPro;

namespace Minesweeper.Runtime.Views
{
    [RequireComponent(typeof(Grid))]
    public class LevelGridView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro tilePrefab;

        [SerializeField] private CellColorSheet colorSheet;

        private Grid _grid;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
        }

        public void DrawLevel(int[,] level)
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
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    int val = level[x, y];
                    string cellText = val < 0 ? "M" : val.ToString();

                    var cellView = Instantiate(tilePrefab, root);
                    cellView.transform.localPosition = _grid.GetCellCenterLocal(new Vector3Int(x, y, 0));
                    cellView.text = cellText;
                    if (cellText != "M")
                        cellView.color = colorSheet.GetColor(val);


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