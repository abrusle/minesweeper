using UnityEngine;

namespace Minesweeper.Runtime.Dev
{
    [RequireComponent(typeof(LevelInputEvents))]
    public class HoverCellTooltip : MonoBehaviour
    {
        public int textSize = 20;
        public Vector2 boxSize = new Vector2(120, 50), offset;
        
        private LevelInputEvents _input;
        private string _tooltipText;
        private Vector2 _position;

        private void Awake()
        {
            enabled = Application.isEditor;
            _input = GetComponent<LevelInputEvents>();
        }

        private void OnEnable()
        {
            _input.OnCellHovered.AddListener(OnCellHovered);
        }

        private void OnDisable()
        {
            _input.OnCellHovered.RemoveListener(OnCellHovered);
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(_position + offset, boxSize), _tooltipText, new GUIStyle(GUI.skin.box)
            {
                fontSize = textSize,
                alignment = TextAnchor.MiddleCenter
            });
        }

        private void LateUpdate()
        {
            _position = UnityEngine.Input.mousePosition;
            _position.y = Screen.height - _position.y;
        }

        private void OnCellHovered(Vector2Int? cellPosition)
        {
            _tooltipText = cellPosition == null ? "null" : cellPosition.Value.ToString();
        }
    }
}