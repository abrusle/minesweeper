using UnityEngine;

namespace Minesweeper.Runtime
{
    public enum VSyncCount : byte
    {
        [InspectorName("Don't Sync (0)")]
        DontSync = 0,
        [InspectorName("1")]
        One = 1,
        [InspectorName("2")]
        Two = 2,
        [InspectorName("3")]
        Three = 3,
        [InspectorName("4")]
        Four = 4
    }
}