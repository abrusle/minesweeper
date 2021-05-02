using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minesweeper.Runtime.Views
{
    public class CellView : MonoBehaviour
    {
        public TextMeshPro textMesh;
        [FormerlySerializedAs("maskingSprite")]
        public SpriteRenderer backgroundSprite;

        public GameObject flagView;

        public Color FlagColor
        {
            set
            {
                var flagSprites = flagView.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var flagSprite in flagSprites)
                {
                    flagSprite.color = value;
                }
            }
        }
    }
}