using UnityEngine;

namespace Minesweeper.Runtime.Experimental.Voronoi
{
    [ExecuteAlways]
    public class MockCursor : MonoBehaviour
    {
        public PointPlacer pointPlacer;

        private Vector3Int? _previousClosestPointCoord;

        private void Update()
        {
            if (pointPlacer == null) return;
            
            var currentPoints = pointPlacer.CurrentPoints;
            Vector3 position = transform.position;
            
            Vector3Int? closestPointCoord = null;
            float minSqrDistance = float.PositiveInfinity;
            Vector3Int currentCell = pointPlacer.Grid.WorldToCell(position);

            foreach (Vector3Int neighbor in pointPlacer.EnumerateCellsInRadius(currentCell, 2))
            {
                Vector3 point = currentPoints[neighbor];
                float sqrDistance = SqrDistance(point, position);
                
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
        
        static float SqrDistance(Vector3 a, Vector3 b)
        {
            float num1 = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float) (num1 * (double) num1 + num2 * (double) num2 + num3 * (double) num3);
        }
    }
}