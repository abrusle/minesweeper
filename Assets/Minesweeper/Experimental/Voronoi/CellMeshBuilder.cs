using System.Collections.Generic;
using Minesweeper.Runtime.Math;
using UnityEngine;

namespace Minesweeper.Runtime.Experimental.Voronoi
{
    public class CellMeshBuilder : MonoBehaviour
    {
        public PointPlacer pointPlacer;

        // debug
        public bool _drawIntersections;
        public List<Line> _lines = new List<Line>();
        
        
        private Vector2Int _currentCellCoords;
        private Vector2Int _previousCellCoords;

        private void OnDrawGizmos()
        {
            if (pointPlacer == null) return;

            Vector2Int newCellCoords = (Vector2Int) pointPlacer.Grid.WorldToCell(transform.position);
            if (newCellCoords != _currentCellCoords)
            {
                _previousCellCoords = _currentCellCoords;
                _currentCellCoords = newCellCoords;
                
                pointPlacer.UnhighlightPoint(_previousCellCoords);
                pointPlacer.HighlightPoint(_currentCellCoords);
            }

            var points = pointPlacer.CurrentPoints;
            if (!points.TryGetValue(_currentCellCoords, out Vector3 pointPosition))
            {
                return;
            }

            if (_drawIntersections)
            {
                _lines.Clear();
            }
            var lines = _lines;
            Vector3 gridForward = pointPlacer.Grid.transform.forward;
            Vector2 cellSize = pointPlacer.Grid.cellSize + pointPlacer.Grid.cellGap;
            foreach (Vector2Int coords in pointPlacer.EnumerateCellsInRadius(_currentCellCoords))
            {
                if (coords == _currentCellCoords)
                    continue;

                if (!points.TryGetValue(coords, out Vector3 position))
                    continue;

                // Gizmos.color = new Color(1, 1, 1, 0.3f);
                // Gizmos.DrawLine(position, pointPosition);
                
                Vector3 midPoint = (pointPosition + position) * 0.5F;
                
                // Gizmos.color = new Color(1, 1, 1, 0.5f);
                // Gizmos.DrawLine(pointPosition, midPoint);

                Vector3 direction = (position - pointPosition).normalized;
                Vector3 normal = Vector3.Cross(direction, gridForward);
                var line = new Line(normal, midPoint);
                
                line.GetSegment(cellSize.x * -pointPlacer.extents.x, cellSize.x * pointPlacer.extents.x, out var start, out var end);
                Gizmos.color = new Color(0.79f, 1f, 0.57f, 0.8f);
                Gizmos.DrawLine(start, end);

                if (!_drawIntersections)
                    continue;
                
                lines.Add(line);
            }

            if (_drawIntersections)
            {
                Gizmos.color = Color.blue;
                for (int i = 0, count = lines.Count; i < count; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        if (j == i) continue;
                        if (lines[i].TryGetIntersection(lines[j], out var intersection))
                        {
                            //Gizmos.DrawIcon(intersection, "hex", false, Gizmos.color);
                            Gizmos.DrawLine(pointPosition, intersection);
                        }
                    }
                }
            }
        }
    }
}