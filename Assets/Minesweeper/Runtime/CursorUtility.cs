using Minesweeper.Runtime.Data;
using UnityEngine;

namespace Minesweeper.Runtime
{
    public static class CursorUtility
    {
        public static void SetCursor(CursorSetup cursorSetup)
        {
            Cursor.SetCursor(cursorSetup.texture, cursorSetup.hotspot, cursorSetup.cursorMode);
        }
    }
}