using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new UI Color Theme", menuName = "Data/UI/Main Menu Color Theme")]
    public class MainMenuColorTheme : ScriptableObject
    {
        public Color Background => background;
        
        public ButtonColors TileButtons => tileButtons;

        public Color Text => text;

        public Color TitleTiles => titleTiles;
        
        [SerializeField] private Color background;
        [SerializeField] private ButtonColors tileButtons;
        [Space(5)]
        [SerializeField] private Color text;
        [SerializeField] private Color titleTiles;
        
        
        [System.Serializable]
        public struct ButtonColors
        {
            public Color normal, highlighted, selected, pressed, disabled;
        }
    }
}