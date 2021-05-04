using System;
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
        
        [Header("Cursor Data")]
        [SerializeField] private CursorSetup normalCursor;
        [SerializeField] private CursorSetup pointerCursor;


        [Header("Cell Highlight")]
        public bool animate = true;
        [FormerlySerializedAs("durationMult")]
        [Min(0.01f)] public float durationMultiplier = 1;
        public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [NonSerialized]
        public Vector2Int positionMin, positionMax;

        private Vector2Int _currentPosition = new Vector2Int(int.MinValue, int.MinValue);
        private bool _isVisible;
        private Coroutine _transitionCoroutine;

        public void UpdatePosition(Vector2Int cellAtCursor)
        {
            if (cellAtCursor == _currentPosition) return;

            _currentPosition = cellAtCursor;
            if (cellAtCursor.x < positionMin.x ||
                cellAtCursor.y < positionMin.y ||
                cellAtCursor.x > positionMax.x ||
                cellAtCursor.y > positionMax.y)
            {
                CursorUtility.SetCursor(normalCursor);
                HideHighlight();
            }
            else if (_isVisible == false || animate == false)
            {
                CursorUtility.SetCursor(pointerCursor);
                ShowHighlight();
            }
            else TransitionHighlight(_currentPosition);
            
        }

        private void HideHighlight()
        {
            if (_isVisible)
                gameObject.SetActive(_isVisible = false);
        }

        private void ShowHighlight()
        {
            gameObject.SetActive(_isVisible = true);
            transform.position = CellToWorld(_currentPosition);
        }

        private void TransitionHighlight(Vector2Int to)
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }

            _transitionCoroutine = StartCoroutine(CoroutineTransitionHighlight(CellToWorld(to)));
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