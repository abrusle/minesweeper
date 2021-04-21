using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Runtime.Animation.UI
{
    public class GraphicColorAnimator : GenericAnimator<Color>
    {
        [SerializeField] private Graphic targetGraphic;

        private void Reset()
        {
            targetGraphic = GetComponent<Graphic>();
        }

        /// <inheritdoc />
        public override Color AnimatedProperty
        {
            get => targetGraphic.color;
            set => targetGraphic.color = value;
        }

        /// <inheritdoc />
        protected override Color LerpProperty(Color from, Color to, float normalizedtime)
        {
            return Color.Lerp(from, to, normalizedtime);
        }
    }
}