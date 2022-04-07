#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    public sealed partial class InfiniteGridGenerator
    {
        [CustomEditor(typeof(InfiniteGridGenerator))]
        private sealed class Inspector : Editor
        {
            private InfiniteGridGenerator _target;
            private void OnEnable()
            {
                _target = (InfiniteGridGenerator)target;
            }

            /// <inheritdoc />
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (Application.isPlaying)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

                    EditorGUILayout.LabelField("Cell Pool Capacity");
                    {
                        Rect progressbarRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 25);
                        int poolCount = _target._cellPool.Count;
                        float poolUsage = poolCount / (float)PoolCapacity;
                        EditorGUI.ProgressBar(progressbarRect, poolUsage, $"{(poolUsage * 100):F1}% ({poolCount}/{PoolCapacity})");
                    }
                    
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Cells in view", _target._cellsInView.Count.ToString());
                }
            }

            /// <inheritdoc />
            public override bool RequiresConstantRepaint()
            {
                return Application.isPlaying;
            }
        }
    }
}
#endif