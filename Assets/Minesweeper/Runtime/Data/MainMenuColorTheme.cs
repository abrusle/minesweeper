using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new UI Color Theme", menuName = "Data/UI/Main Menu Color Theme")]
    public class MainMenuColorTheme : ScriptableObject
    {
        public Color Background => background;

        public Color TileButtonHighlight => tileButtonHighlight;

        public Color TileButtonNormal => tileButtonNormal;

        public Color Text => text;

        public Color TitleTiles => titleTiles;
        
        [SerializeField] private Color 
            background,
            tileButtonNormal,
            tileButtonHighlight,
            text,
            titleTiles;
        
    }
}