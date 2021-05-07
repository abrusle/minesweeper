using UnityEngine;

namespace Minesweeper.Runtime
{
    [RequireComponent(typeof(Camera))]
    public class GameCameraClassic : MainCamera
    {
        [SerializeField] private Vector2 margin;

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

            Camera.orthographicSize = orthoSize * 0.5f;
        }
    }
}