using System.Text;
using UnityEngine;

namespace Minesweeper.Runtime.Noise
{
    [CreateAssetMenu(fileName = "new Noise Sampler", menuName = "Funtional SO/Noise/Sampler")]
    public class NoiseSampler : ScriptableObject
    {
        public int minValue, maxValue;
        
        public int SampleAt(Vector2Int pos)
        {
            var rand = new System.Random(pos.GetHashCode());
            return rand.Next(minValue, maxValue);
        }

        public void SmapleMany(Vector2Int startPos, int[,] noise)
        {
            var endPos = new Vector2Int
            {
                x = startPos.x + noise.GetLength(0),
                y = startPos.y + noise.GetLength(1)
            };

            for (int x = startPos.x; x < endPos.x; x++)
            {
                for (int y = startPos.y; y < endPos.y; y++)
                {
                    noise[x, y] = SampleAt(new Vector2Int(x, y));
                }
            }
        }

        private void TestSample()
        {
            minValue = 3; maxValue = 26;
            
            var noise = new int[8, 8];
            SmapleMany(Vector2Int.zero, noise);

            var sb = new StringBuilder();
            for (var x = 0; x < 8; x++)
            {
                sb.AppendFormat("\n{0} : ", x);
                for (int y = 0; y < 8; y++)
                {
                    sb.Append(noise[x, y] + ", ");
                }
            }

            Debug.Log(sb);
        }
    }
}