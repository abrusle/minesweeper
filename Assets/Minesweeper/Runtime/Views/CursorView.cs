using System.Collections;
using Minesweeper.Runtime.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minesweeper.Runtime.Views
{
    public class CursorView : MonoBehaviour
    {
        [Header("Scene Dependencies")]
        public Grid grid;
        public LevelInputEvents inputEvents;
        public SpriteRenderer view;
        
        [Header("Cursor Data")]
        [SerializeField] private CursorSetup normalCursor;
        [SerializeField] private CursorSetup pointerCursor;


        [Header("Cell Highlight")]
        public bool animate = true;
        [FormerlySerializedAs("durationMult")]
        [Min(0.01f)] public float durationMultiplier = 1;
        public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private bool _isVisible;
        private Coroutine _transitionCoroutine;

        private void OnEnable()
        {
            inputEvents.OnCellHovered.AddListener(OnCellHovered);
        }

        private void OnDisable()
        {
            inputEvents.OnCellHovered.RemoveListener(OnCellHovered);
        }

        private void OnCellHovered(Vector2Int? cellPos)
        {
            Debug.Log(cellPos == null ? "null" : cellPos.Value.ToString());
            if (cellPos == null) // Out of bounds
            {
                CursorUtility.SetCursor(normalCursor);
                HideHighlight();
            }
            else if (_isVisible == false || animate == false) // insde bounds but was hidden previously
            {
                CursorUtility.SetCursor(pointerCursor);
                ShowHighlight(cellPos.Value);
            }
            else TransitionHighlight(cellPos.Value); // inside bounds -> inside bounds
        }

        public void UpdatePosition(Vector2Int cellAtCursor)
        {
            /*
            if (cellAtCursor == _currentPosition) return;

            _currentPosition = cellAtCursor;
            if (cellAtCursor.x < positionMin.x ||
                cellAtCursor.y < positionMin.y ||
                cellAtCursor.x > positionMax.x ||
                cellAtCursor.y > positionMax.y) // Out of bounds
            {
                CursorUtility.SetCursor(normalCursor);
                HideHighlight();
            }
            else if (_isVisible == false || animate == false) // insde bounds but was hidden previously
            {
                CursorUtility.SetCursor(pointerCursor);
                ShowHighlight(_currentPosition);
            }
            else TransitionHighlight(_currentPosition); // inside bounds -> inside bounds
            */
        }

        private void HideHighlight()
        {
            if (_isVisible)
                view.gameObject.SetActive(_isVisible = false);
            StopTransition();
        }

        private void ShowHighlight(Vector2Int position)
        {
            StopTransition();
            view.gameObject.SetActive(_isVisible = true);
            transform.position = CellToWorld(position);
        }

        private void TransitionHighlight(Vector2Int to)
        {
            StopTransition();
            _transitionCoroutine = StartCoroutine(CoroutineTransitionHighlight(CellToWorld(to)));
        }

        private void StopTransition()
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }
        }

        private IEnumerator CoroutineTransitionHighlight(Vector3 to)
        {
            var from = transform.position;

            for (float t = 0; t < 1; t += Time.deltaTime * durationMultiplier)
            {
                transform.position = Vector3.Lerp(from, to, ease.Evaluate(t));
                yield return null;
            }

            transform.position = to;
            _transitionCoroutine = null;
        }

        private Vector3 CellToWorld(Vector2Int cellPos)
        {
            return grid.CellToWorld(cellPos.AddZ(0)) + (grid.cellSize + grid.cellGap) * 0.5f;
        }
    }
}