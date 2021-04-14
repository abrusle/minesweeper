using Minesweeper.Runtime.Views.UI.Animation;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.EditorGUI;

namespace Minesweeper.Editor
{
    /// <summary>
    /// Left to do:
    /// - Check transition duplicates
    /// - Check transition to same state
    /// - Recover from unequal array sizes.
    ///
    /// Bonus:
    /// - Detect click on a transition
    ///     - Allow duplicating and deleting selected transition
    /// </summary>
    [CustomEditor(typeof(MainMenuAnimationStateMachine))]
    internal class MainMenuAnimationStateMachineEditor : UnityEditor.Editor
    {

        private static readonly Color InvalidColor = new Color(1f, 0.36f, 0.36f);
        
        private struct Properties
        {
            public SerializedProperty
                startState,
                transitions,
                clips;
        }

        private struct Contents
        {
            public GUIContent
                animationClip,
                addButton;
        }

        private Properties _properties;
        private Contents _contents;
        private bool _cancelCurrentTransitionDrawLoop;

        private void OnEnable()
        {
            _properties = new Properties
            {
                startState = serializedObject.FindProperty("startState"),
                transitions = serializedObject.FindProperty("transitions"),
                clips = serializedObject.FindProperty("transitionClips")
            };

            _contents = new Contents
            {
                animationClip = new GUIContent("Animation Clip"),
                addButton = new GUIContent("+", "Add Transition")
            };
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            PropertyField(_properties.startState);
            Separator();
            LabelField("Transitions", EditorStyles.whiteLargeLabel);

            _cancelCurrentTransitionDrawLoop = false;
            int transitionCount = _properties.transitions.arraySize;
            for (int i = 0; i < transitionCount; i++)
            {
                if (_cancelCurrentTransitionDrawLoop) break;
                DrawTransition(i);
                if (i < transitionCount - 1)
                    Separator();
            }

            Separator();
            using (new HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(_contents.addButton, GUILayout.Width(50))) OnAddButtonClick();
            }
            
        }

        private void OnAddButtonClick()
        {
            _properties.transitions.AddEmptyArrayElement();
            _properties.clips.AddEmptyArrayElement();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTransition(int transitionIndex)
        {
            SerializedProperty transitionProp = _properties.transitions.GetArrayElementAtIndex(transitionIndex);
            using (new HorizontalScope())
            {
                using (new VerticalScope(EditorStyles.helpBox))
                {
                    using (new HorizontalScope())
                    {
                        LabelField(transitionIndex.ToString(), GUILayout.Width(20));
                        DrawState(transitionIndex, transitionProp, "from");
                        LabelField("→", GUILayout.Width(14));
                        DrawState(transitionIndex, transitionProp, "to");
                    }

                    var clipProp = _properties.clips.GetArrayElementAtIndex(transitionIndex);
                    var initialClip = clipProp.objectReferenceValue;
                    var newClip = ObjectField(_contents.animationClip, initialClip, typeof(AnimationClip), false);
                    if (newClip != initialClip)
                    {
                        clipProp.objectReferenceValue = newClip;
                        OnTransitionPropertiesChanged(transitionIndex);
                    }
                }

                if (GUILayout.Button("x", GUILayout.ExpandHeight(true), GUILayout.Width(20)))
                    RemoveTransitionAtIndex(transitionIndex);
            }
        }

        private void DrawState(int transitionIndex, SerializedProperty transitionProp, string statePropPath)
        {
            var fromProp = transitionProp.FindPropertyRelative(statePropPath);
            var initialFromValue = (MainMenuState) fromProp.enumValueIndex;
            var newFromValue = (MainMenuState) EnumPopup(initialFromValue);
            if (initialFromValue != newFromValue)
            {
                fromProp.enumValueIndex = (int) newFromValue;
                OnTransitionPropertiesChanged(transitionIndex);
            }
        }

        private void RemoveTransitionAtIndex(int transitionIndex)
        {
            _cancelCurrentTransitionDrawLoop = true;
            _properties.transitions.DeleteArrayElementAtIndex(transitionIndex);
            _properties.clips.DeleteArrayElementAtIndex(transitionIndex);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnTransitionPropertiesChanged(int transitionIndex)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}