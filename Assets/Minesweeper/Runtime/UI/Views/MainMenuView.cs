using System.Collections;
using Minesweeper.Runtime.Animation;
using Minesweeper.Runtime.Data;
using Minesweeper.Runtime.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Runtime.UI.Views
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Animation animator;
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private Button classicButton;
        [SerializeField] private Button infiniteButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button backButton;
        [SerializeField] private MainMenuAnimationStateMachine stateMachine;
        [SerializeField] private MainMenuThemeMap themeMap;
        [SerializeField] private ColorThemeApplier themeApplier;

        private Coroutine _animationWaitCoroutine;

        private void Start()
        {
            stateMachine.ResetState();
            BroadcastStateChange(stateMachine.CurrentState, stateMachine.CurrentState);
            themeApplier.Theme = themeMap[stateMachine.CurrentState];
        }

        private void OnEnable()
        {
            quitButton.onClick.AddListener(OnQuitButtonClick);
            classicButton.onClick.AddListener(OnClassicButtonClick);
            infiniteButton.onClick.AddListener(OnInfiniteButtonClick);
            settingsButton.onClick.AddListener(OnSettingsButtonClick);
            backButton.onClick.AddListener(OnBackButtonClick);
        }
        
        private void OnDisable()
        {
            quitButton.onClick.RemoveListener(OnQuitButtonClick);
            classicButton.onClick.RemoveListener(OnClassicButtonClick);
            infiniteButton.onClick.RemoveListener(OnInfiniteButtonClick);
            settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
            backButton.onClick.RemoveListener(OnBackButtonClick);
        }

        private void OnBackButtonClick()
        {
            SwitchState(MainMenuState.Default);
        }

        private void OnSettingsButtonClick()
        {
            SwitchState(MainMenuState.Settings);
        }

        private void OnInfiniteButtonClick()
        {
            SwitchState(MainMenuState.Infinite);
        }

        private void OnClassicButtonClick()
        {
            SwitchState(MainMenuState.Classic);
        }

        public bool SwitchState(MainMenuState state)
        {
            if (stateMachine.CurrentState == state) return false;
            if (!stateMachine.CanTransitionTo(state)) return false;

            var previousState = stateMachine.CurrentState;
            var transitionClip = stateMachine.TransitionTo(state);
            PlayAnimation(transitionClip);
            themeApplier.Theme = themeMap[stateMachine.CurrentState];

            backButton.interactable = state != MainMenuState.Default;
            classicButton.interactable = state == MainMenuState.Default;
            infiniteButton.interactable = state == MainMenuState.Default;
            settingsButton.interactable = state == MainMenuState.Default;
            quitButton.interactable = state == MainMenuState.Default;

            BroadcastStateChange(state, previousState);

            return true;
        }

        private void PlayAnimation(AnimationClip clip)
        {
            graphicRaycaster.enabled = false;
            if (animator.isPlaying)
                animator.Stop();
            animator.RemoveClip(animator.clip);
            animator.clip = clip;
            animator.AddClip(clip, clip.name);
            animator.Play();
            Debug.Log("Playing " + clip.name, clip);

            StopAnimationEndAwaiter();
            _animationWaitCoroutine = StartCoroutine(WaitForAnimationEnd(clip));
        }

        private IEnumerator WaitForAnimationEnd(AnimationClip clip)
        {
            yield return new WaitForSeconds(clip.length);
            OnAnimationComplete();
            _animationWaitCoroutine = null;
        }

        private void StopAnimationEndAwaiter()
        {
            if (_animationWaitCoroutine != null)
            {
                StopCoroutine(_animationWaitCoroutine);
                _animationWaitCoroutine = null;
            }
        }

        private void OnAnimationComplete()
        {
            graphicRaycaster.enabled = true;
            Debug.Log("Animation Complete");
        }

        private void OnQuitButtonClick()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
            #else
            Application.Quit();
            #endif
        }

        private void BroadcastStateChange(MainMenuState state, MainMenuState previousState)
        {
            foreach (var listener in GetComponentsInChildren<IMainMenuStateChangeListener>())
            {
                listener.OnMainMenuStateChange(previousState, state);
            }
        }
    }
}