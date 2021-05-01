using System;
using UnityEngine;

namespace Minesweeper.Runtime
{
    public class InputHandler : MonoBehaviour
    {
        public static event Action LeftClick, RightClick, FullScreenToggle;
        
        private static InputHandler _Instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (_Instance != null) return;

            _Instance = new GameObject(nameof(InputHandler)).AddComponent<InputHandler>();
            DontDestroyOnLoad(_Instance);
        }
        
        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
                return;
            }
            
            if (_Instance != this) Destroy(this);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                LeftClick?.Invoke();
            }

            if (Input.GetMouseButtonUp(1))
            {
                RightClick?.Invoke();
            }

            if (Input.GetKeyUp(KeyCode.F11))
            {
                FullScreenToggle?.Invoke();
            }
        }
    }
}