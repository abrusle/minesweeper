using System;
using System.Linq;
using UnityEngine;

namespace Minesweeper.Runtime.Data
{
    [CreateAssetMenu(fileName = "new Cell Color Sheet", menuName = "Data/Cell Color Sheet", order = 0)]
    public class ColorSheet : ScriptableObject
    {
        [Header("Cells")]
        public ColorRule[] rules;
        [Space]
        public Color mineColor;
        public Color unrevealedColor;
        public Color revealedColor;
        
        [Header("World")]
        public Color bgColor;
        public Color gameOverBgColor;
        public Color gameWonBgColor;

        [Serializable]
        public struct ColorRule
        {
            public int threshold;
            public Color color;
        }

        public Color GetColor(int value)
        {
            rules = rules.OrderBy(r => r.threshold).ToArray();
            foreach (var rule in rules)
            {
                if (value <= rule.threshold) return rule.color;
            }

            return rules[rules.Length - 1].color;
        }
    }
}