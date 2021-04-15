using UnityEditor;
using UnityEngine;

namespace Minesweeper.Editor
{
    public static class PropertyUtility
    {
        public static void AddEmptyArrayElement(this SerializedProperty prop)
        {
            prop.InsertArrayElementAtIndex(Mathf.Max(0, prop.arraySize - 1));
        }
    }
}