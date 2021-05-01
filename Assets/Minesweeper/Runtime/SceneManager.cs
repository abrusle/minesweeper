using Minesweeper.Runtime.Data;
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

        [SerializeField] private SerializableDictionary<SceneId, SceneSettings> sceneSettings;

        public void SwitchToGameClassic() => SwitchScene(SceneId.GameClassic);
        public void SwitchToMainMenu() => SwitchScene(SceneId.MainMenu);
        public void SwitchToGameInfinite() => SwitchScene(SceneId.GameInfinite);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnApplicationLoad()
        {
            var currentScene = (SceneId) GetActiveScene().buildIndex;
            Instance.sceneSettings[currentScene].ApplyNow();
        }
        
        public void SwitchScene(SceneId sceneId)
        {
            LoadScene((int) sceneId, LoadSceneMode.Single);
            sceneSettings[sceneId].ApplyNow();
        }
    }
}