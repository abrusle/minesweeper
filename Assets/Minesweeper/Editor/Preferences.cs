using UnityEditor;

namespace Minesweeper.Editor
{
    public static class Preferences
    {
        private const string KeyRegenerateSeedOnAwakeForInfiniteMode = "Minesweeper.Editor.Infinite.RegenSeedOnAwake";
        public static bool RegenerateSeedOnAwakeForInfiniteMode
        {
            get => EditorPrefs.GetBool(KeyRegenerateSeedOnAwakeForInfiniteMode, false);
            set => EditorPrefs.SetBool(KeyRegenerateSeedOnAwakeForInfiniteMode, value);
        }
    }
}