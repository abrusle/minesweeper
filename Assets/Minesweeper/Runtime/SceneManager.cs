using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minesweeper.Runtime
{
    using static UnityEngine.SceneManagement.SceneManager;
    
    [AssetPath("Components/SceneManager")]
    public class SceneManager : ScriptableSingleton<SceneManager>
    {
        public enum SceneId
        {
            MainMenu = 0,
            GameClassic = 1,
            GameInfinite = 2
        }

        public void SwitchToGameClassic() => SwitchScene(SceneId.GameClassic);
        public void SwitchToMainMenu() => SwitchScene(SceneId.MainMenu);
        public void SwitchToGameInfinite() => SwitchScene(SceneId.GameInfinite);
        
        public void SwitchScene(SceneId sceneId)
        {
            LoadScene((int) sceneId, LoadSceneMode.Single);
        }
    }
}