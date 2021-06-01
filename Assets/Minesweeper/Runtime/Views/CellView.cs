using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minesweeper.Runtime.Views
{
    public class CellView : MonoBehaviour
    {
        public enum State
        {
            AtRest,
            Selected,
            InReveal
        }
        
        public TextMeshPro textMesh;
        [FormerlySerializedAs("maskingSprite")]
        public SpriteRenderer backgroundSprite;

        [SerializeField] private Animator flagAnimator;
        [SerializeField] private Animator mainAnimator;

        private readonly struct FlagAnimatorProperties
        {
            public static readonly int IsVisible = Animator.StringToHash("IsVisible");
        }

        private readonly struct MainAnimatorParameters
        {
            public static readonly int Selected = Animator.StringToHash("Selected");
            public static readonly int Reveal = Animator.StringToHash("Reveal");
        }
        
        public void ToggleFlag(bool active)
        {
            flagAnimator.SetBool(FlagAnimatorProperties.IsVisible, active);
        }

        private SpriteRenderer[] _flagSprites;

        private void Awake()
        {
            _flagSprites = flagAnimator.GetComponentsInChildren<SpriteRenderer>(true);
        }

        private void Start()
        {
            flagAnimator.Play("Flag pop out", 0, 1);
            flagAnimator.SetBool(FlagAnimatorProperties.IsVisible, false);
        }

        public Color FlagColor
        {
            set
            {
                foreach (var flagSprite in _flagSprites)
                    flagSprite.color = value;
            }
        }

        public void SetState(State state)
        {
            switch (state)
            {
                case State.AtRest:
                    mainAnimator.SetBool(MainAnimatorParameters.Selected, false);
                    break;
                case State.Selected:
                    mainAnimator.SetBool(MainAnimatorParameters.Selected, true);
                    break;
                case State.InReveal:
                    mainAnimator.SetTrigger(MainAnimatorParameters.Reveal);
                    mainAnimator.SetBool(MainAnimatorParameters.Selected, false);
                    break;
            }
        }
    }
}