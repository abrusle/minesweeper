using System.Collections;
using Minesweeper.Runtime.Animation;
using Minesweeper.Runtime.UI.MainMenu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Minesweeper.Runtime.UI.Views
{
    public class InfiniteModeLaunchView : UIBehaviour, IMainMenuStateChangeListener
    {
        [Header("References")]
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Canvas canvas;

        [Space]
        [SerializeField] private Button playButton;

        [Header("Settings")]
        [SerializeField, Range(0, 1)] private float animationIntervalDuration = 0.1f;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            playButton.onClick.AddListener(OnPlayButtonClick);
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            playButton.onClick.RemoveListener(OnPlayButtonClick);
        }

        private void OnPlayButtonClick()
        {
            SceneManager.Instance.SwitchToGameInfinite();
        }
        
        /// <inheritdoc />
        public void OnMainMenuStateChange(MainMenuState from, MainMenuState to)
        {
            bool isInfinite = to == MainMenuState.Infinite;
            graphicRaycaster.enabled = isInfinite;
            canvasGroup.interactable = isInfinite;

            if (isInfinite)
            {
                Fade(true);
            }
            else if (from == MainMenuState.Infinite)
            {
                Fade(false);
            }
        }
        private void Fade(bool visible)
        {
            if (_transitionAnim != null)
            {
                StopCoroutine(_transitionAnim);
            }

            _transitionAnim = StartCoroutine(TransitionAnimationCoroutine(visible, animationIntervalDuration));
        }

        private Coroutine _transitionAnim;

        private IEnumerator TransitionAnimationCoroutine(bool visible, float interval)
        {
            float targetAlpha = visible ? 1 : 0;
            float startAlpha = canvasGroup.alpha;

            if (visible)
            {
                canvas.enabled = true;
            }
            
            for (float t = 0; t < 1; t += interval)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            if (!visible)
            {
                canvas.enabled = false;
            }
        }
    }
}