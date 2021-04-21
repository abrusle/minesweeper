using Minesweeper.Runtime.Animation;
using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new Main Menu Theme Map", menuName = "Data/UI/Main Menu/Theme Map")]
    public class MainMenuThemeMap : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<MainMenuState, MainMenuColorTheme> themeMap;

        public MainMenuColorTheme this[MainMenuState state] => themeMap[state];

    }
}