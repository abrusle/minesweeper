using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new Level Settings", menuName = "Data/Level Settings", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        public Vector2Int size;
        public int mineCount;

        private void OnValidate()
        {
            if (mineCount > size.x * size.y)
                mineCount = size.x * size.y;
        }
    }
}