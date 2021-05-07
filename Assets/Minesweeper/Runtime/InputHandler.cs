using System;
using UnityEngine;

namespace Minesweeper.Runtime
{
    public class InputHandler : MonoBehaviour
    {
        public static event Action
            MouseLeftUp,
            MouseRightUp,
            MouseLeftDown,
            MouseRightDown,
            FullScreenToggle;
        

        public static Vector3 MousePositionWorld
        {
            get => _Instance._cachedMousePositionWorld 
                   ?? (_Instance._cachedMousePositionWorld = _Instance.GetMousePositionWorld()).Value;
        }
        
        private static InputHandler _Instance;
        private Vector3? _cachedMousePositionWorld;

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
                MouseLeftUp?.Invoke();
            else if (Input.GetMouseButtonDown(0))
                MouseLeftDown?.Invoke();

            if (Input.GetMouseButtonUp(1))
                MouseRightUp?.Invoke();
            else if (Input.GetMouseButtonDown(1))
                MouseRightDown?.Invoke();

            if (Input.GetKeyUp(KeyCode.F11))
                FullScreenToggle?.Invoke();
        }

        private void LateUpdate()
        {
            _cachedMousePositionWorld = null;
        }

        private Vector3 GetMousePositionWorld()
        {
            return MainCamera.Instance.Camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}