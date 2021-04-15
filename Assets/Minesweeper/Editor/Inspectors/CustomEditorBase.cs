using UnityEditor;

namespace Minesweeper.Editor.Inspectors
{
    internal abstract class CustomEditorBase : UnityEditor.Editor
    {
        protected void Reimport()
        {
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
        }
    }
}