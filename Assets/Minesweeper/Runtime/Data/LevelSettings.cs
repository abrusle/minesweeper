using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new Level Settings", menuName = "Data/Level Settings", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        public int rowCount, columnCount, mineCount;
    }
}