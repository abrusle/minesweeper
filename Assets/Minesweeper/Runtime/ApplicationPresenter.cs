using Minesweeper.Runtime.Input;
using UnityEngine;

namespace Minesweeper.Runtime
{
    [AssetPath("Components/Application Presenter")]
    public class ApplicationPresenter : ScriptableSingleton<ApplicationPresenter>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnApplicationLoad()
        {
            GeneralActions.ToggleFullscreen += Instance.OnFullscreenToggle;
        }

        private void OnFullscreenToggle()
        {
            if (!Screen.fullScreen)
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
            else
                Screen.fullScreen = false;
            
            Debug.Log("Fullscreen " + Screen.currentResolution);
        }
    }
}