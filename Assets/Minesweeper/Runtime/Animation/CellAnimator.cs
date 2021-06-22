using UnityEngine;

namespace Minesweeper.Runtime.Animation
{
    public static class CellAnimatorHelper
    {
        public readonly struct Parameters
        {
            public static readonly int Selected = Animator.StringToHash("Selected");
            public static readonly int Reveal = Animator.StringToHash("Reveal");
        }
        
        public enum State
        {
            AtRest,
            Selected,
            InReveal
        }
        
        public static void SetState(State state, Animator baseAnimator)
        {
            switch (state)
            {
                case State.AtRest:
                    baseAnimator.SetBool(Parameters.Selected, false);
                    break;
                case State.Selected:
                    baseAnimator.SetBool(Parameters.Selected, true);
                    break;
                case State.InReveal:
                    baseAnimator.SetTrigger(Parameters.Reveal);
                    baseAnimator.SetBool(Parameters.Selected, false);
                    break;
            }
        }
    }
}