using System;
using JetBrains.Annotations;
using Minesweeper.Runtime.Data;
using Minesweeper.Runtime.Input;
using UnityEngine;
using UnityEngine.Events;

namespace Minesweeper.Runtime
{
    public class LevelInputEvents : LevelActions
    {
        /// <summary>
        /// Invoked when the mouse hovers over a new cell.
        /// </summary>
        public UnityEvent<Vector2Int?> OnCellHovered => onCellHovered;

        public IClickEventEmitter NormalClick => normalClick;
        
        public IClickEventEmitter SpecialClick => specialClick;
        
        [SerializeField] private new Camera camera;
        [SerializeField] private Grid levelGrid;
        [SerializeField, CanBeNull] private LevelSettings levelSettings;
        [SerializeField] private KeyCode frameKey = KeyCode.F;

        [Space]
        [SerializeField, Tooltip("Invoked when the mouse hovers over a new cell.")]
        private UnityEvent<Vector2Int?> onCellHovered;
        
        [SerializeField, Tooltip("Move the camera to frame the entire level area.")]
        private UnityEvent onFrameAction;

        private Vector2Int _currentCursorGridPos;
        private Vector3 _freeCursorPosition;
        private bool _withinLevelBounds;

        [SerializeField] private ClickTracker normalClick, specialClick;
        
        private void OnEnable()
        {
            normalClick.Enabled = true;
            specialClick.Enabled = true;
            GeneralActions.Cancel += OnCancelKey;
        }

        private void OnDisable()
        {
            normalClick.Enabled = false;
            specialClick.Enabled = false;
            GeneralActions.Cancel -= OnCancelKey;
        }

        /// <inheritdoc />
        protected override void Update()
        {
            _freeCursorPosition = UnityEngine.Input.mousePosition;

            // TODO handle cursor leaving and entering window
            var newCursorPos = (Vector2Int) levelGrid.WorldToCell(camera.ScreenToWorldPoint(_freeCursorPosition));
            if (newCursorPos != _currentCursorGridPos)
            {
                _withinLevelBounds = levelSettings == null || LevelUtility.IsCellWithinBounds(newCursorPos, levelSettings.size);
                _currentCursorGridPos = newCursorPos;
                normalClick.currentCursorGridPos = specialClick.currentCursorGridPos = _currentCursorGridPos;
                if (enabled)
                {
                    onCellHovered.Invoke(_withinLevelBounds ? _currentCursorGridPos : null);
                }

                normalClick.OnCellHovered(_currentCursorGridPos);
                specialClick.OnCellHovered(_currentCursorGridPos);
            }

            base.Update();

            if (UnityEngine.Input.GetKeyUp(frameKey))
            {
                onFrameAction.Invoke();
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

        private void OnCancelKey()
        {
            normalClick.CancelSelection();
            specialClick.CancelSelection();
        }

        private bool CanEmitEvents()
        {
            return _withinLevelBounds;
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
            /// <inheritdoc/>
            public UnityEvent<Vector2Int> OnSelected => onSelected;
            
            /// <inheritdoc/>
            public UnityEvent<Vector2Int> OnClicked => onClicked;
            
            /// <inheritdoc/>
            public UnityEvent<Vector2Int> OnDeselected => onDeselected;

            public bool Enabled { get; set; } = true;
            
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
                    if (Enabled) onDeselected?.Invoke(_selectedCell.Value);
                    _selectedCell = null;
                }
            }

            public void OnPress()
            {
                if (Enabled) onSelected?.Invoke(currentCursorGridPos);
                _selectedCell = currentCursorGridPos;
            }

            public void OnRelease()
            {
                if (_selectedCell == null) return;

                if (_selectedCell.Value == currentCursorGridPos)
                {
                    if (Enabled) onClicked?.Invoke(currentCursorGridPos);
                }
                
                if (Enabled) onDeselected?.Invoke(currentCursorGridPos);
                _selectedCell = null;
            }

            public void CancelSelection()
            {
                if (_selectedCell == null)
                    return;
                
                if (Enabled) onDeselected?.Invoke(currentCursorGridPos);
                _selectedCell = null;
            }
        }
    }
}