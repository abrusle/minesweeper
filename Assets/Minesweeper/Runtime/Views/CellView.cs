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
    }
}