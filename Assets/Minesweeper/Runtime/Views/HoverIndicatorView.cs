using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minesweeper.Runtime.Views
{
    public class HoverIndicatorView : MonoBehaviour
    {
        public Grid grid;
        public bool animate = true;
        [FormerlySerializedAs("durationMult")]
        [Min(0.01f)] public float durationMultiplier = 1;
        
        [NonSerialized]
        public Vector2Int positionMin, positionMax;

        private Vector2Int _currentPosition = new Vector2Int(int.MinValue, int.MinValue);
        private bool _isVisible;
        private Coroutine _transitionCoroutine;

        public void UpdatePosition(Vector2Int cellAtCursor)
        {
            if (cellAtCursor == _currentPosition) return;

            var previous = _currentPosition;
            _currentPosition = cellAtCursor;
            if (cellAtCursor.x < positionMin.x ||
                cellAtCursor.y < positionMin.y ||
                cellAtCursor.x > positionMax.x ||
                cellAtCursor.y > positionMax.y)
            {
                HideCursor();
            }
            else if (_isVisible == false || animate == false)
            {
                ShowCursor();
            }
            else TransitionCursor(previous, _currentPosition);
            
        }

        private void HideCursor()
        {
            if (_isVisible != false)
                gameObject.SetActive(_isVisible = false);
        }

        private void ShowCursor()
        {
            gameObject.SetActive(_isVisible = true);
            transform.position = CellToWorld(_currentPosition);
        }

        private void TransitionCursor(Vector2Int from, Vector2Int to)
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }

            // Debug.Log($"transition start from {from} to {to}");
            _transitionCoroutine = StartCoroutine(CoroutineTransition(CellToWorld(to)));
        }

        private IEnumerator CoroutineTransition(Vector3 to)
        {
            var from = transform.position;

            for (float t = 0; t < 1; t += Time.deltaTime * durationMultiplier)
            {
                transform.position = Vector3.Lerp(from, to, t);
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