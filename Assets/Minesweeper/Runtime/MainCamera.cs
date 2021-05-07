using UnityEngine;

namespace Minesweeper.Runtime
{
    [RequireComponent(typeof(Camera)), DisallowMultipleComponent]
    public class MainCamera : MonoBehaviour
    {
        public static MainCamera Instance { get; private set; }
        
        public Camera Camera => _cam ? _cam : _cam = GetComponent<Camera>();
        private Camera _cam;

        protected virtual void Awake()
        {
            Instance = this;
        }

        public static implicit operator Camera(MainCamera main)
        {
            return main.Camera;
        }
    }
}