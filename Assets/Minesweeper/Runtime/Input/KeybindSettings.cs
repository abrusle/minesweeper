using UnityEngine;

namespace Minesweeper.Runtime.Input
{
    [CreateAssetMenu(fileName = "new Keybind Settings", menuName = "Data/Input/Keybind Settings")]
    public class KeybindSettings : ScriptableDictionary<UserAction, KeyCode>
    {
        
    }
}