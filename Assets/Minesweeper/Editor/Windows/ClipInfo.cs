namespace Minesweeper.Editor.Windows
{
    using UnityEditor;
    using UnityEngine;

    // Editor window for listing all float curves in an animation clip
    public class ClipInfo : EditorWindow
    {
        private AnimationClip _clip;

        [MenuItem("Window/Animation/Clip Info")]
        static void Init()
        {
            GetWindow(typeof(ClipInfo));
        }

        public void OnGUI()
        {
            _clip = EditorGUILayout.ObjectField("Clip", _clip, typeof(AnimationClip), false) as AnimationClip;

            EditorGUILayout.LabelField("Curves:");
            if (_clip != null)
            {
                foreach (var binding in AnimationUtility.GetCurveBindings(_clip))
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_clip, binding);
                    EditorGUILayout.LabelField(binding.path + "/" + binding.propertyName + ", Keys: " + curve.keys.Length);
                }
            }
        }
    }
}