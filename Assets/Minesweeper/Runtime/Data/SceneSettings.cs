using UnityEngine;
using UnityEngine.Serialization;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new Scene Settings", menuName = "Data/Scene Settings")]
    public class SceneSettings : ScriptableObject
    {
        [FormerlySerializedAs("taretFrameRate")] [Min(-1)]
        public int targetFrameRate;
        public VSyncCount vSyncCount;

        public void ApplyNow()
        {
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = (int) vSyncCount;

            Debug.Log("Applied " + this, this);
        }
    }
}