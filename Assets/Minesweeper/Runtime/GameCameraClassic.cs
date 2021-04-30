using UnityEngine;

namespace Minesweeper.Runtime
{
    [RequireComponent(typeof(Camera))]
    public class GameCameraClassic : MonoBehaviour
    {
        public Camera Camera => _camera;


        [SerializeField] private Vector2 margin;
        
        
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        public void FitToLevel(Vector2 levelSize, Grid grid)
        {
            Vector2 cellSize = grid.cellSize;
            Vector2 cellGap = grid.cellGap;
            Vector2 levelSizeWorldSpace = (cellSize + cellGap) * levelSize - cellGap + margin;

            float screenAspect = Screen.width / (float) Screen.height;
            float levelAspect = levelSizeWorldSpace.x / levelSizeWorldSpace.y;

            float orthoSize = levelSizeWorldSpace.y;
            
            if (screenAspect < levelAspect)
                orthoSize *= levelAspect / screenAspect;

            _camera.orthographicSize = orthoSize * 0.5f;
        }

        public static implicit operator Camera(GameCameraClassic classic)
        {
            return classic.Camera;
        }
    }
}