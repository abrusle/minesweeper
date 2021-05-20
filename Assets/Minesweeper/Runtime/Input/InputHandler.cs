using UnityEngine;

namespace Minesweeper.Runtime.Input
{
    public class InputHandler : MonoBehaviour
    {
        public delegate void KeyChangeDelegate(bool isPressed);
        public delegate void InputActionEventDelegate();

        protected KeybindSettings keybindings;

        protected virtual void Awake()
        {
            keybindings = Resources.Load<KeybindSettings>("Input/Keybind Settings");
        }

        protected void HandleAction(UserAction action, KeyChangeDelegate keyEvent)
        {
            if (UnityEngine.Input.GetKeyDown(keybindings[action]))
                keyEvent?.Invoke(true);
            else if (UnityEngine.Input.GetKeyUp(keybindings[action]))
                keyEvent?.Invoke(false);
        }

        protected void HandleActionSimple(UserAction action, InputActionEventDelegate keyEvent)
        {
            if (UnityEngine.Input.GetKeyUp(keybindings[action]))
                keyEvent?.Invoke();
        }
    }
}