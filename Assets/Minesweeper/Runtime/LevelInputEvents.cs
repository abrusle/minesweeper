using UnityEngine;
using UnityEngine.Events;

namespace Minesweeper.Runtime
{
    public class LevelInputEvents : InputSystemInterpreter
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private Grid levelGrid;

        /// <summary>
        /// Invoked when the mouse hovers over a new cell.
        /// </summary>
        [Space]
        [SerializeField, Tooltip("Invoked when the mouse hovers over a new cell.")]
        private UnityEvent<Vector2Int?> onCellHovered;

        /// <summary>
        /// Invoked when the mouse button is pressed down.
        /// </summary>
        [SerializeField, Tooltip("Invoked when the mouse button is pressed down.")]
        private UnityEvent<Vector2Int> onCellSelected;
        
        /// <summary>
        /// Invoked when the mouse button is released.
        /// </summary>
        [SerializeField, Tooltip("Invoked when the mouse button is released.")]
        private UnityEvent<Vector2Int> onCellDeselected;

        /// <summary>
        /// Invoked when a cell is clicked.
        /// </summary>
        [SerializeField, Tooltip("Invoked when a cell is clicked.")]
        private UnityEvent<Vector2Int> onCellClicked;

        /// <summary>
        /// Invoked when the mouse button is pressed down.
        /// </summary>
        [SerializeField, Tooltip("Invoked when the mouse button is pressed down.")]
        private UnityEvent<Vector2Int> onBeforeSpecialClick;
        
        /// <summary>
        /// Invoked when the mouse button is pressed down.
        /// </summary>
        [SerializeField, Tooltip("Invoked when the mouse button is pressed down.")]
        private UnityEvent<Vector2Int> onSpecialClick;

        private Vector2Int _currentCursorGridPos;
        private Vector2Int? _selectedCell, _specialSelectedCell;
        
        // TODO take level bounds into account
        // TODO handle cursor leaving window
        // TODO add ESC key to cancel selection
        // TODO check both clicks pressed at the same time.

        /// <inheritdoc />
        protected override void Update()
        {
            base.Update();
            var newCursorPos = (Vector2Int) levelGrid.WorldToCell(camera.ScreenToWorldPoint(FreeCursorPosition));
            if (newCursorPos != _currentCursorGridPos)
            {
                _currentCursorGridPos = newCursorPos;
                onCellHovered.Invoke(_currentCursorGridPos);
                
                if (_selectedCell != null && _selectedCell.Value != newCursorPos)
                {
                    onCellDeselected.Invoke(_selectedCell.Value);
                    _selectedCell = null;
                }
            }
        }

        /// <inheritdoc />
        protected override void OnClickPrimary(bool pressed)
        {
            if (pressed) // button pressed
            {
                onCellSelected.Invoke(_currentCursorGridPos);
                _selectedCell = _currentCursorGridPos;
            }
            else if (_selectedCell != null)// button released
            {
                if (_selectedCell.Value == _currentCursorGridPos)
                {
                    onCellClicked.Invoke(_currentCursorGridPos);
                }

                onCellDeselected.Invoke(_selectedCell.Value);
                _selectedCell = null;
            }
        }

        /// <inheritdoc />
        protected override void OnClickSecondary(bool pressed)
        {
            // same behavior as primary click but with different events and selectedCell.
        }
    }
}