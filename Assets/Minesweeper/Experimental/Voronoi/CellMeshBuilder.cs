
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper.Runtime.Experimental.Voronoi
{
    public class CellMeshBuilder : MonoBehaviour
    {
        public PointPlacer pointPlacer;
        public Vector3Int cellCoords;

        private readonly List<Vector3Int> _cellCoordsList = new();
        private bool _isDirty;

        private Vector3Int _previousCellCoords;
        
        private void OnEnable()
        {
            _isDirty = true;
        }

        private void OnValidate()
        {
            _isDirty = true;
        }

        private void OnDrawGizmos()
        {
            if (pointPlacer == null) return;

            var points = pointPlacer.CurrentPoints;
            if (!points.TryGetValue(cellCoords, out Vector3 pointPosition))
            {
                return;
            }
            
            if (_isDirty)
            {
                pointPlacer.UnhighlightPoint(_previousCellCoords);
                pointPlacer.HighlightPoint(cellCoords);

                _previousCellCoords = cellCoords;
            }

            Vector3 gridForward = pointPlacer.Grid.transform.forward;
            foreach (Vector3Int coords in pointPlacer.EnumerateCellsInRadius(cellCoords, 2))
            {
                if (coords == cellCoords)
                    continue;

                if (!points.TryGetValue(coords, out Vector3 position))
                    continue;

                // Gizmos.color = new Color(1, 1, 1, 0.3f);
                // Gizmos.DrawLine(position, pointPosition);
                
                Vector3 midPoint = (pointPosition + position) * 0.5F;
                
                // Gizmos.color = new Color(1, 1, 1, 0.5f);
                // Gizmos.DrawLine(pointPosition, midPoint);

                Vector3 direction = (position - pointPosition).normalized;
                Vector3 weightedNormalDirection = Vector3.Cross(direction, gridForward) * 2.1f;
                
                Gizmos.color = new Color(0.79f, 1f, 0.57f, 0.8f);
                Gizmos.DrawLine(midPoint + weightedNormalDirection, midPoint - weightedNormalDirection);
            }
        }
    }
}