using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    [CreateAssetMenu(fileName = "new Time Settings", menuName = "Data/Infinite Mode/Time Settinsg")]
    public class TimeSettings : ScriptableObject
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        [field: Min(0), SerializeField] public float BaseDuration { get; private set; }
        [field: Min(0), SerializeField] public float BoostDuration { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}