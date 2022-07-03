using Minesweeper.Runtime.Math;
using UnityEngine;

namespace Minesweeper.Runtime.Experimental.Voronoi
{
    [ExecuteAlways]
    public class MockCursor : MonoBehaviour
    {
        public PointPlacer pointPlacer;

        private Vector2Int? _previousClosestPointCoord;

        private void Update()
        {
            if (pointPlacer == null) return;
            
            var currentPoints = pointPlacer.CurrentPoints;
            Vector3 position = transform.position;
            
            Vector2Int? closestPointCoord = null;
            float minSqrDistance = float.PositiveInfinity;
            Vector2Int currentCell = (Vector2Int)pointPlacer.Grid.WorldToCell(position);

            foreach (Vector2Int neighbor in pointPlacer.EnumerateCellsInRadius(currentCell))
            {
                Vector3 point = currentPoints[neighbor];
                float sqrDistance = MathUtility.SqrDistance(point, position);
                
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    closestPointCoord = neighbor;
                }
            }

            if (_previousClosestPointCoord != closestPointCoord)
            {
                if (_previousClosestPointCoord.HasValue)
                {
                    pointPlacer.UnhighlightPoint(_previousClosestPointCoord.Value);
                }
                
                _previousClosestPointCoord = closestPointCoord;
                if (closestPointCoord.HasValue)
                {
                    pointPlacer.HighlightPoint(closestPointCoord.Value);
                }
            }

            if (closestPointCoord.HasValue)
            {
                Debug.DrawLine(position, currentPoints[closestPointCoord.Value], Color.yellow);
            }
        }
    }
}