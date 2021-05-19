using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Minesweeper.Runtime.Input
{
    public class GeneralActions : InputHandler
    {
        public static event InputActionEventDelegate ToggleFullscreen, Cancel;

        #region Singleton
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnApplicationLoad()
        {
            _Instance = new GameObject("Input - General Actions").AddComponent<GeneralActions>();
            _Instance.name = "General Actions";
            DontDestroyOnLoad(_Instance.gameObject);
        }
        
        private static GeneralActions _Instance;
        
        #endregion

        /// <inheritdoc />
        protected override void Awake()
        {
            if (_Instance != null && _Instance != this)
            {
                Destroy(this);
                return;
            }

            base.Awake();
        }

        private void Update()
        {
            HandleActionSimple(UserAction.ToggleFullscreen, ToggleFullscreen);
            HandleActionSimple(UserAction.Cancel, Cancel);
        }
    }
}