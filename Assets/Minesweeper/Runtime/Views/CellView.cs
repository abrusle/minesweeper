using TMPro;
using UnityEngine;

namespace Minesweeper.Runtime.Views
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMesh;
        [SerializeField] private SpriteRenderer maskingSprite;

        private void Start()
        {
            textMesh.enabled = false;
        }

        public Color TextColor
        {
            get => textMesh.color;
            set => textMesh.color = value;
        }

        public string Text
        {
            get => textMesh.text;
            set => textMesh.text = value;
        }

        public void Reveal()
        {
            textMesh.enabled = true;
        }

        public void PlaceFlag()
        {
            maskingSprite.color = Color.yellow;
        }
    }
}