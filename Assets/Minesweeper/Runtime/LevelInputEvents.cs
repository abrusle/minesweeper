using System;
using Minesweeper.Runtime.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Minesweeper.Runtime
{
    public class LevelInputEvents : InputSystemInterpreter
    {
        /// <summary>
        /// Invoked when the mouse hovers over a new cell.
        /// </summary>
        public UnityEvent<Vector2Int?> OnCellHovered => onCellHovered;

        public IClickEventEmitter NormalClick => normalClick;
        
        public IClickEventEmitter SpecialClick => specialClick;
        
        [SerializeField] private new Camera camera;
        [SerializeField] private Grid levelGrid;
        [SerializeField] private LevelSettings levelSettings;
        

        [Space]
        [SerializeField, Tooltip("Invoked when the mouse hovers over a new cell.")]
        private UnityEvent<Vector2Int?> onCellHovered;

        private Vector2Int _currentCursorGridPos;
        private bool _currentlyWithinLevelBounds;

        [SerializeField] private ClickTracker normalClick, specialClick;

        // TODO add ESC key to cancel selection

        /// <inheritdoc />
        protected override void Update()
        {
            base.Update();
            // TODO handle cursor leaving and entering window
            var newCursorPos = (Vector2Int) levelGrid.WorldToCell(camera.ScreenToWorldPoint(FreeCursorPosition));
            if (newCursorPos != _currentCursorGridPos)
            {
                bool withinLevelBounds = LevelUtility.IsCellWithinBounds(newCursorPos, levelSettings.size);
                _currentCursorGridPos = newCursorPos;
                normalClick.currentCursorGridPos = specialClick.currentCursorGridPos = _currentCursorGridPos;
                if (_currentlyWithinLevelBounds || withinLevelBounds)
                    onCellHovered.Invoke(withinLevelBounds ? _currentCursorGridPos : (Vector2Int?) null);

                _currentlyWithinLevelBounds = withinLevelBounds;

                normalClick.OnCellHovered(_currentCursorGridPos);
                specialClick.OnCellHovered(_currentCursorGridPos);
            }
        }

        /// <inheritdoc />
        protected override void OnClickPrimary(bool pressed)
        {
            if (CanEmitEvents() == false)
                return;
            
            if (pressed)
                normalClick.OnPress();
            else
                normalClick.OnRelease();
        }

        /// <inheritdoc />
        protected override void OnClickSecondary(bool pressed)
        {
            if (CanEmitEvents() == false)
                return;

            if (pressed)
                specialClick.OnPress();
            else
                specialClick.OnRelease();
        }

        private bool CanEmitEvents()
        {
            return LevelUtility.IsCellWithinBounds(_currentCursorGridPos, levelSettings.size);
            // TODO check if click is over a UI object.
        }

        public interface IClickEventEmitter
        {
            UnityEvent<Vector2Int> OnSelected { get; }
            UnityEvent<Vector2Int> OnClicked { get; }
            UnityEvent<Vector2Int> OnDeselected { get; }
        }

        [Serializable]
        private class ClickTracker : IClickEventEmitter
        {
            public UnityEvent<Vector2Int> OnSelected => onSelected;
            public UnityEvent<Vector2Int> OnClicked => onClicked;
            public UnityEvent<Vector2Int> OnDeselected => onDeselected;
            
            [NonSerialized]
            public Vector2Int currentCursorGridPos;
            
            [SerializeField] private UnityEvent<Vector2Int> onSelected;
            [SerializeField] private UnityEvent<Vector2Int> onClicked;
            [SerializeField] private UnityEvent<Vector2Int> onDeselected;
            
            private Vector2Int? _selectedCell;

            public void OnCellHovered(Vector2Int hoverCell)
            {
                if (_selectedCell != null && _selectedCell.Value != hoverCell)
                {
                    onDeselected?.Invoke(_selectedCell.Value);
                    _selectedCell = null;
                }
            }

            public void OnPress()
            {
                onSelected?.Invoke(currentCursorGridPos);
                _selectedCell = currentCursorGridPos;
            }

            public void OnRelease()
            {
                if (_selectedCell == null) return;

                if (_selectedCell.Value == currentCursorGridPos)
                {
                    onClicked?.Invoke(currentCursorGridPos);
                }
                
                onDeselected?.Invoke(currentCursorGridPos);
                _selectedCell = null;
            }
        }
    }
}