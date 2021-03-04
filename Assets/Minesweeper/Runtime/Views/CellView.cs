using TMPro;
using UnityEngine;

namespace Minesweeper.Runtime.Views
{
    public class CellView : MonoBehaviour
    {
        public Vector2Int GridPosition { get; private set; }

        [SerializeField] private TextMeshPro textMesh;
        [SerializeField] private SpriteRenderer maskingSprite;

        public Color TextColor
        {
            get => textMesh.color;
            set => textMesh.color = value;
        }

        public void Load(int x, int y, string value)
        {
            GridPosition = new Vector2Int(x, y);
            textMesh.text = value;
            textMesh.enabled = false;
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