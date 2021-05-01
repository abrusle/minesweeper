using Abrusle.CorouTween;
using UnityEngine;
using UpdateAnimationDelegate = Abrusle.CorouTween.TweeningUtility.UpdateAnimationDelegate;

namespace Minesweeper.Runtime.Animation.UI
{
    public abstract class GenericAnimator<TValue> : MonoBehaviour
    {
        private Tween _tween;
        
        public abstract TValue AnimatedProperty { get; set; }

        public void PlayAnimation(TValue to, float duration, AnimationCurve ease = null)
        {
            PlayAnimation(AnimatedProperty, to, duration, ease);
        }
        
        public void PlayAnimation(TValue from, TValue to, float duration, AnimationCurve ease = null)
        {
            if (_tween != null && _tween.IsPlaying)
                _tween.Stop();

            _tween = new Tween(this,
                ease == null 
                    ? (UpdateAnimationDelegate) UpdateWithoutEase 
                    : UpdateWithEase, 
            duration);
            
            _tween.Play(onComplete: () =>
            {
                ease = null;
                _tween = null;
            });

            void UpdateWithEase(float t)
            { 
                AnimatedProperty = LerpProperty(from, to, ease.Evaluate(t));
            }

            void UpdateWithoutEase(float t)
            {
                AnimatedProperty = LerpProperty(from, to, t);
            }
        }

        public void StopAnimation()
        {
            if (_tween != null && _tween.IsPlaying)
            {
                _tween.Stop();
                _tween = null;
            }
        }

        protected abstract TValue LerpProperty(TValue from, TValue to, float normalizedtime);
    }
}