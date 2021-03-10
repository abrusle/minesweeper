using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new Cursor", menuName = "Data/Cursor Setup", order = 0)]
    public class CursorSetup : ScriptableObject
    {
        public Texture2D texture;
        public Vector2 hotspot;
        public CursorMode cursorMode;
    }
}