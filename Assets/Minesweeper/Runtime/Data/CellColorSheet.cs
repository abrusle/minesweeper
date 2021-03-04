using System;
using System.Linq;
using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new Cell Color Sheet", menuName = "Data/Cell Color Sheet", order = 0)]
    public class CellColorSheet : ScriptableObject
    {
        public ColorRule[] rules;
        
        [Serializable]
        public struct ColorRule
        {
            public int threshold;
            public Color color;
        }

        public Color GetColor(int value)
        {
            rules = rules.OrderBy(r => r.threshold).ToArray();
            for (int i = 0; i < rules.Length; i++)
            {
                var rule = rules[i];
                if (value <= rule.threshold) return rule.color;
            }

            return rules[rules.Length - 1].color;
        }
    }
}