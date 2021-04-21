using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Runtime.Animation.UI
{
    public struct ButtonColorData
    {
        public Color? normal, highlighted, selected, pressed, disabled;
    }
    public class ButtonColorAnimator : GenericAnimator<ButtonColorData>
    {
        [SerializeField] private Button targetButton;

        private void Reset()
        {
            targetButton = GetComponent<Button>();
        }

        /// <inheritdoc />
        public override ButtonColorData AnimatedProperty
        {
            get => new ButtonColorData
            {
                normal = targetButton.colors.normalColor,
                highlighted = targetButton.colors.highlightedColor,
                selected = targetButton.colors.selectedColor,
                pressed = targetButton.colors.pressedColor,
                disabled = targetButton.colors.disabledColor
            };
            set
            {
                var colors = targetButton.colors;
                colors.normalColor = value.normal ?? targetButton.colors.normalColor;
                colors.highlightedColor = value.highlighted ?? targetButton.colors.highlightedColor;
                colors.selectedColor = value.selected ?? targetButton.colors.selectedColor;
                colors.pressedColor = value.pressed ?? targetButton.colors.pressedColor;
                colors.disabledColor = value.disabled ?? targetButton.colors.disabledColor;
                targetButton.colors = colors;
            }
        }

        /// <inheritdoc />
        protected override ButtonColorData LerpProperty(ButtonColorData from, ButtonColorData to, float normalizedtime)
        {
            return new ButtonColorData
            {
                normal      = LerpNullableColor(from.normal     , to.normal     , normalizedtime),
                highlighted = LerpNullableColor(from.highlighted, to.highlighted, normalizedtime),
                selected    = LerpNullableColor(from.selected   , to.selected   , normalizedtime),
                pressed     = LerpNullableColor(from.pressed    , to.pressed    , normalizedtime),
                disabled    = LerpNullableColor(from.disabled   , to.disabled   , normalizedtime)
            };
        }

        private static Color? LerpNullableColor(Color? a, Color? b, float t)
        {
            if (a == null || b == null) return null;

            return Color.Lerp(a.Value, b.Value, t);
        }
    }
}