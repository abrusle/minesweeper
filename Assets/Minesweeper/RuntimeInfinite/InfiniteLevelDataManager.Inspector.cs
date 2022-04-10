#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    public sealed partial class InfiniteLevelDataManager
    {
        [CustomEditor(typeof(InfiniteLevelDataManager))]
        private sealed class Inspector : Editor
        {
            private InfiniteLevelDataManager _target;

            private void OnEnable()
            {
                _target = (InfiniteLevelDataManager)target;
            }

            /// <inheritdoc />
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
                BoundsInt bounds = _target.Bounds;
                EditorGUILayout.LabelField(nameof(Bounds));
                EditorGUI.indentLevel++;
                GUI.enabled = false;
                EditorGUILayout.Vector2IntField("Min",  (Vector2Int)bounds.min);
                EditorGUILayout.Vector2IntField("Max",  (Vector2Int)bounds.max);
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif