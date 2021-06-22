using Minesweeper.Runtime.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minesweeper.Runtime.Views
{
    public class CellView : MonoBehaviour
    {
        public TextMeshPro textMesh;
        [FormerlySerializedAs("maskingSprite")]
        public SpriteRenderer backgroundSprite;

        [SerializeField] private Animator flagAnimator;
        [SerializeField] private Animator mainAnimator;

        private readonly struct FlagAnimatorProperties
        {
            public static readonly int IsVisible = Animator.StringToHash("IsVisible");
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

        public void SetState(CellAnimatorHelper.State state)
        {
            CellAnimatorHelper.SetState(state, mainAnimator);
        }
    }
}