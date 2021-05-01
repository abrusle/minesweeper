using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new Scene Settings", menuName = "Data/Scene Settings")]
    public class SceneSettings : ScriptableObject
    {
        [Min(-1)]
        public int taretFrameRate;
        public VSyncCount vSyncCount;

        public void ApplyNow()
        {
            Application.targetFrameRate = taretFrameRate;
            QualitySettings.vSyncCount = (int) vSyncCount;

            Debug.Log("Applied " + this, this);
        }
    }
}