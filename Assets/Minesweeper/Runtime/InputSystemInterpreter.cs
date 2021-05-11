using UnityEngine;
using UnityEngine.InputSystem;

namespace Minesweeper.Runtime
{
    public abstract class InputSystemInterpreter : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActionAsset;

        private InputActionMap _inGameActionMap;
        protected InGameInputActions InGameActions { get; private set; }
        protected GlobalInputActions GlobalActions { get; private set; }
        protected Vector2 FreeCursorPosition { get; private set; }

        protected virtual void OnEnable()
        {
            _inGameActionMap = inputActionAsset.FindActionMap("InGame", true);
            _inGameActionMap.Enable();

            InGameActions = new InGameInputActions(
                _inGameActionMap.FindAction("Click Primary", true),
                _inGameActionMap.FindAction("Click Secondary", true),
                _inGameActionMap.FindAction("Free Cursor Position", true));
            
            InGameActions.clickPrimary.performed += OnClickPrimaryPerformed;
            InGameActions.clickSecondary.performed += OnClickSecondaryPerformed;
        }

        protected virtual void OnDisable()
        {
            InGameActions.clickPrimary.performed -= OnClickPrimaryPerformed;
            
            _inGameActionMap.Disable();
        }

        protected virtual void Update()
        {
            FreeCursorPosition = InGameActions.freeCursorPosition.ReadValue<Vector2>();
        }

        private void OnClickPrimaryPerformed(InputAction.CallbackContext context)
        {
            OnClickPrimary(context.ReadValueAsButton());
        }

        private void OnClickSecondaryPerformed(InputAction.CallbackContext context)
        {
            OnClickSecondary(context.ReadValueAsButton());
        }

        protected virtual void OnClickPrimary(bool pressed) {}
        protected virtual void OnClickSecondary(bool pressed) {}

        protected readonly struct GlobalInputActions
        {
            public readonly InputAction cancel, toggleFullScreen;
        }

        protected readonly struct InGameInputActions
        {
            public readonly InputAction
                clickPrimary,
                clickSecondary,
                freeCursorPosition;

            public InGameInputActions(InputAction clickPrimary,
                InputAction clickSecondary,
                InputAction freeCursorPosition)
            {
                this.clickPrimary = clickPrimary;
                this.clickSecondary = clickSecondary;
                this.freeCursorPosition = freeCursorPosition;
            }
        }
    }
}