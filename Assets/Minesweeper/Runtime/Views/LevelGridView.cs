using System.Text;
using UnityEngine;

namespace Minesweeper.Runtime.Views
{
    public class LevelGridView : MonoBehaviour
    {
        public void DrawLevel(int[,] level)
        {
            var sb = new StringBuilder("Generated Level:\n");

            for (int x = 0; x < level.GetLength(0); x++)
            {
                for (int y = 0; y < level.GetLength(1); y++)
                {
                    int val = level[x, y];
                    
                    sb.Append(val < -2 ? "M" : val.ToString()).Append(", ");
                }

                sb.Append('\n');
            }
            
            Debug.Log(sb.ToString());
        }
    }
}