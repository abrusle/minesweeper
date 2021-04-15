using Minesweeper.Runtime.Animation;
using UnityEditor;
using UnityEngine;

namespace Minesweeper.Editor.Inspectors.Specific
{
    [CustomEditor(typeof(AnimationClipReverser))]
    internal class AnimationClipReverserEditor : GenericAssetModifierEditor<AnimationClip, AnimationClip>
    {
        /// <inheritdoc />
        protected override void ModifyAsset(AnimationClip source, AnimationClip result)
        {
            AnimationUtility.SetAnimationClipSettings(result, AnimationUtility.GetAnimationClipSettings(source));
            DeleteAllCurves(result);

            float durationSeconds = source.length;
            
            EditorCurveBinding[] srcBindings = AnimationUtility.GetCurveBindings(source);
            AnimationCurve[] modifiedCurves = new AnimationCurve[srcBindings.Length];
            
            for (var curveIndex = 0; curveIndex < srcBindings.Length; curveIndex++)
            {
                AnimationCurve srcCurve = AnimationUtility.GetEditorCurve(source, srcBindings[curveIndex]);
                Keyframe[] flippedKeys = new Keyframe[srcCurve.length];

                for (int keyIndex = 0; keyIndex < srcCurve.length; keyIndex++)
                {
                    flippedKeys[keyIndex] = FlipKeyframe(srcCurve.keys[keyIndex], durationSeconds);
                }

                modifiedCurves[curveIndex] = new AnimationCurve(flippedKeys)
                {
                    postWrapMode = srcCurve.postWrapMode,
                    preWrapMode = srcCurve.preWrapMode
                };
            }

            AnimationUtility.SetEditorCurves(result, srcBindings, modifiedCurves);
        }

        private static Keyframe FlipKeyframe(Keyframe srcKeyframe, float clipDuration)
        {
            return new Keyframe
            {
                value = srcKeyframe.value,
                time = clipDuration - srcKeyframe.time,
                inTangent = srcKeyframe.outTangent,
                outTangent = srcKeyframe.inTangent,
                inWeight = srcKeyframe.outWeight,
                outWeight = srcKeyframe.inWeight,
                weightedMode = srcKeyframe.weightedMode,
#pragma warning disable 618 // Obsolete
                tangentMode = srcKeyframe.tangentMode
#pragma warning restore 618 // Obsolete
            };
        }

        private static void DeleteAllCurves(AnimationClip clip)
        {
            EditorCurveBinding[] resultBindings = AnimationUtility.GetCurveBindings(clip);
            foreach (var binding in resultBindings)
            {
                AnimationUtility.SetEditorCurve(clip, binding, null);
            }
        }

        /// <inheritdoc />
        protected override AnimationClip CreateAssetPreModification(AnimationClip source)
        {
            var res = Instantiate(source);
            res.name = source.name + " (Reversed)";
            return res;
        }
    }
}