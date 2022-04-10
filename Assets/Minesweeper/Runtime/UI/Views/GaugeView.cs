using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Minesweeper.Runtime.UI.Views
{
    [DisallowMultipleComponent, ExecuteAlways]
    public class GaugeView : UIBehaviour, ILayoutController
    {
        [PublicAPI] public RectTransform.Axis Axis
        {
            get => axis;
            set
            {
                if (axis == value) return;
                axis = value;
                SetDirty();
            }
        }
        
        [PublicAPI] public float Level
        {
            get => level;
            set
            {
                if (Mathf.Approximately(value, level))
                    return;
                
                level = Mathf.Clamp01(value);
                SetDirty();
            }
        }

        [PublicAPI] public RectTransform LevelTransform
        {
            get => levelTransform;
            set
            {
                if (levelTransform == value) return;
                levelTransform = value;
                SetDirty();
            }
        }

        // ReSharper disable once InconsistentNaming
        [PublicAPI] public RectTransform rectTransform => (RectTransform)transform;

        [SerializeField] private RectTransform.Axis axis;
        [SerializeField] private bool invert;
        [SerializeField, Range(0,1)] private float level;
        [SerializeField] private RectTransform levelTransform;

        [NonSerialized] private DrivenRectTransformTracker _tracker;
        
        [PublicAPI] public void SetDirty()
        {
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        /// <inheritdoc />
        public void SetLayoutHorizontal()
        {
            if (IsActive() && axis == RectTransform.Axis.Horizontal && levelTransform != null)
            {
                SetLayout();
            }
        }

        /// <inheritdoc />
        public void SetLayoutVertical()
        {
            if (IsActive() && axis == RectTransform.Axis.Vertical && levelTransform != null)
            {
                SetLayout();
            }
        }

        private void SetLayout()
        {
            _tracker.Clear();
            Vector2 anchorMax = Vector2.one;
            Vector2 anchorMin = Vector2.zero;

            if (invert)
            {
                anchorMax[(int)axis] = level;
            }
            else
            {
                anchorMin[(int)axis] = 1 - level;
            }
            
            levelTransform.anchorMax = anchorMax;
            levelTransform.anchorMin = anchorMin;

            _tracker.Add(this, levelTransform, DrivenTransformProperties.Anchors);
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
        #endif
    }
}