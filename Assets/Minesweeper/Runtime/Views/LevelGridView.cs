using System.Collections;
using UnityEngine;

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
        [SerializeField, Min(0)] private float spawnAnimationInterval;
        
        private CellView[,] _cellViews;
        private AnimationSequence<RevealAnimationDatum> _revealSequence;
        private Coroutine _drawLevelOperation;

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
        
        public void SetCellFlag(int x, int y, bool active)
        {
            var cellView = _cellViews[x,y];
            cellView.backgroundSprite.color = active ? colorSheet.flaggedCellColor : colorSheet.unrevealedCellColor;
            cellView.FlagColor = active ? colorSheet.flagColor : colorSheet.GetColor(0);
            cellView.ToggleFlag(active);
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
            if (_drawLevelOperation !=  null)
                StopCoroutine(_drawLevelOperation);
            
            Clear();

            var root = new GameObject($"Level ({xMax} x {yMax})").transform;
            root.SetParent(Grid.transform);
            root.localPosition = Vector3.zero;
            
            _cellViews = new CellView[xMax, yMax];
            _drawLevelOperation = StartCoroutine(InstantiateAsync());

            IEnumerator InstantiateAsync()
            {
                var interval = new WaitForSeconds(spawnAnimationInterval);

                var iMax = new Vector2Int(xMax - 1, yMax - 1);
                int nMax = iMax.x + iMax.y;
                for (int n = 0; n <= nMax; n++)
                {
                    for (int i = 0; i <= n; i++)
                    {
                        var coord = new Vector2Int(n - i, i);
                        if (coord.x > iMax.x || coord.y > iMax.y)
                            continue;
                        InstantiateCell(root, coord.x, coord.y);
                    }

                    yield return interval;
                }

                _drawLevelOperation = null;
            }
            
            MoveLevelCenterToWorldOrigin(xMax, yMax);
        }

        private void InstantiateCell(Transform root, int x, int y)
        {
            var cellView = Instantiate(cellViewPrefab, root);
            cellView.name = $"Cell [{x}; {y}]";
            cellView.transform.localPosition = Grid.GetCellCenterLocal(new Vector3Int(x, y, 0));
            _cellViews[x, y] = cellView;
            cellView.textMesh.text = string.Empty;
            cellView.textMesh.enabled = false;
            cellView.backgroundSprite.color = colorSheet.unrevealedCellColor;
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
            var sizeAndGap = (Grid.cellSize + Grid.cellGap);
            sizeAndGap.Scale(gridPos);
            Grid.transform.position = sizeAndGap * 0.5f;
        }

        private void PlayRevealAnimation(RevealAnimationDatum datum)
        {
            CellView cellView = _cellViews[datum.x, datum.y];
            
            cellView.SetState(CellAnimatorHelper.State.InReveal);

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

        public void SelectCell(int x, int y)
        {
            _cellViews[x, y].SetState(CellAnimatorHelper.State.Selected);
        }

        public void DeselectCell(int x, int y)
        {
            _cellViews[x, y].SetState(CellAnimatorHelper.State.AtRest);
        }
    }
}