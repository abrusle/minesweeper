using System;
using UnityEngine;

namespace Minesweeper.Runtime.Views.UI.Animation
{
    public abstract class AnimationStateMachineBase : ScriptableObject
    {
        
    }
    public abstract class AnimationStateMachine<TState> : ScriptableObject where TState : struct, Enum
    {
        public TState CurrentState { get; private set; }
        
        [Serializable]
        public struct Transition
        {
            public TState from, to;

            public Transition(TState from, TState to)
            {
                this.from = from;
                this.to = to;
            }
        }

        [SerializeField] private TState startState;

        [SerializeField] private Transition[] transitions;
        [SerializeField] private AnimationClip[] transitionClips;

        public void ResetState()
        {
            CurrentState = startState;
        }

        public AnimationClip TransitionTo(TState state)
        {
            var clip = GetAnimationClip(new Transition(CurrentState, state));
            if (clip == null)
                throw new StateMachineException($"Transition from {CurrentState} to {state}");
            CurrentState = state;
            return clip;
        }

        public bool CanTransitionTo(TState state)
        {
            return HasTransition(new Transition(CurrentState, state));
        }

        public bool HasTransition(Transition transition)
        {
            return Array.IndexOf(transitions, transition) >= 0;
        }

        protected AnimationClip GetAnimationClip(Transition transition)
        {
            int index = Array.IndexOf(transitions, transition);
            if (index < 0) return null;
            return transitionClips[index];
        }

        protected class StateMachineException : Exception
        {
            public StateMachineException(string message) : base(message)
            {
                
            }
        }
    }
}