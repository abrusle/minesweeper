using UnityEditor;

namespace Minesweeper.Editor
{
    public static class PropertyUtility
    {
        public static void AddEmptyArrayElement(this SerializedProperty prop)
        {
            prop.InsertArrayElementAtIndex(prop.arraySize - 1);
        }
    }
}