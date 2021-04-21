using Minesweeper.Runtime.Animation.UI;
using Minesweeper.Runtime.Data;
using UnityEngine;

namespace Minesweeper.Runtime.UI.MainMenu
{
    public class ColorThemeApplier : MonoBehaviour
    {
        [System.Serializable]
        private struct Targets
        {
            public GraphicColorAnimator[]
                background,
                text,
                titleTile;

            public ButtonColorAnimator[]
                tileButton;
        }


        public MainMenuColorTheme Theme
        {
            get => theme;
            set
            {
                theme = value;
                StartTransition();
            }
        }

        [SerializeField] private MainMenuColorTheme theme;
        [Min(0.01f)]
        [SerializeField] private float transitionDuration;
        [SerializeField] private AnimationCurve transitionEase;
        [SerializeField] private Targets targetComponents;
        
        private void StartTransition()
        {
            foreach (var targetAnimator in targetComponents.background)
                targetAnimator.PlayAnimation(theme.Background, transitionDuration, transitionEase);
            
            foreach (var targetAnimator in targetComponents.text)
                targetAnimator.PlayAnimation(theme.Text, transitionDuration, transitionEase);
            
            foreach (var targetAnimator in targetComponents.titleTile)
                targetAnimator.PlayAnimation(theme.TitleTiles, transitionDuration, transitionEase);

            var buttonColors = new ButtonColorData
            {
                normal = theme.TileButtonNormal,
                highlighted = theme.TileButtonHighlight,
                pressed = theme.TileButtonHighlight,
                selected = theme.TileButtonHighlight,
                disabled = theme.TileButtonNormal
            };
            
            foreach (var buttonAnimator in targetComponents.tileButton)
                buttonAnimator.PlayAnimation(buttonColors, transitionDuration, transitionEase);
        }

        private void Reset()
        {
            transitionDuration = 0.5f;
            transitionEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
    }
}