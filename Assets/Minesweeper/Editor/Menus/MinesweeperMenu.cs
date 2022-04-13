using UnityEditor;

namespace Minesweeper.Editor.Menus
{
    internal static class MinesweeperMenu
    {
        private const string BaseMenuPath = "Minesweeper/";

        private static class InfiniteMenu
        {
            public const string MenuPath = BaseMenuPath + "Infinite/";

            private const string PathSeedRegen = MenuPath + "Auto Regenerate Seed";
            
            [MenuItem(PathSeedRegen, true)]
            private static bool ValidateToggleSeedRegenerationOnAwake()
            {
                Menu.SetChecked(PathSeedRegen, Preferences.RegenerateSeedOnAwakeForInfiniteMode);
                return true;
            }
            
            [MenuItem(PathSeedRegen)]
            private static void ToggleSeedRegenerationOnAwake()
            {
                var isOn = !Preferences.RegenerateSeedOnAwakeForInfiniteMode;
                Preferences.RegenerateSeedOnAwakeForInfiniteMode = isOn;
                Menu.SetChecked(PathSeedRegen, isOn);
            }
        }
    }
}