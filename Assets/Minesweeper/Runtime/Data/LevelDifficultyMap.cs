using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new Level Difficulty Map", menuName = "Data/Level Difficulty Map")]
    public sealed class LevelDifficultyMap : ScriptableDictionary<Difficulty, LevelSettings>
    {
        
    }
}