using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Minesweeper.Runtime.Animation
{
    public class AnimationSequence<TData> : IDisposable
    {
        public delegate void PlayAnimationDelegate(TData animationData);
        
        public int IntervalMiliseconds
        {
            get => _intervalMiliseconds;
            set => _intervalMiliseconds = Mathf.Max(0, value);
        }
        
        private int _intervalMiliseconds;
        private bool _unstacking;
        
        private readonly Queue<TData> _animationQueue;
        private readonly PlayAnimationDelegate _onPlayAnimation;

        public AnimationSequence([NotNull] PlayAnimationDelegate onPlayAnimation)
        {
            if (onPlayAnimation == null)
                throw new ArgumentNullException(nameof(onPlayAnimation));
            
            _animationQueue = new Queue<TData>();
            _onPlayAnimation = onPlayAnimation;
        }

        public void AddAnimation(TData animationData)
        {
            if (_intervalMiliseconds < 1)
            {
                _onPlayAnimation(animationData);
                return;
            }
            _animationQueue.Enqueue(animationData);
            if (!_unstacking) UnstackProgressively();
        }

        private async void UnstackProgressively()
        {
            _unstacking = true;
            while (_animationQueue.Count > 0)
            {
                _onPlayAnimation(_animationQueue.Dequeue());
                await Task.Delay(_intervalMiliseconds);
            }

            _unstacking = false;
        }

        public void Dispose()
        {
            _animationQueue.Clear();
        }
    }
}