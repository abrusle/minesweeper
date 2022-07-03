using System.Collections.Generic;
using Minesweeper.Runtime.Math;
using UnityEngine;

namespace Minesweeper.Runtime.Experimental.Voronoi
{
    [ExecuteAlways]
    public class CellMeshBuilder : MonoBehaviour
    {
        public PointPlacer pointPlacer;

        public bool 
            drawInner = true,
            drawOuter = true,
            drawMidLines = true;

        // debug
        private List<Line> _lines = new ();
        private List<IntersectionData> _intersectionPoints = new();
        private Vector3 _currentCellPoint;

        private Vector2Int _currentCellCoords;
        private Vector2Int _previousCellCoords;
        private bool _meshRebuildNeeded;

        private void Update()
        {
            if (pointPlacer != null)
            {
                Vector2Int newCellCoords = (Vector2Int) pointPlacer.Grid.WorldToCell(transform.position);
                if (newCellCoords != _currentCellCoords)
                {
                    _previousCellCoords = _currentCellCoords;
                    _currentCellCoords = newCellCoords;
                    
                    pointPlacer.UnhighlightPoint(_previousCellCoords);
                    pointPlacer.HighlightPoint(_currentCellCoords);

                    _meshRebuildNeeded = true;
                }
            }
        }

        private void LateUpdate()
        {
            if (_meshRebuildNeeded)
            {
                BuildMeshData(_currentCellCoords);
                _meshRebuildNeeded = false;
            }
        }

        private void BuildMeshData(Vector2Int centerCellCoords)
        {
            var points = pointPlacer.CurrentPoints;
            if (!points.TryGetValue(centerCellCoords, out Vector3 pointPosition))
            {
                return;
            }

            _currentCellPoint = pointPosition;
            
            _lines.Clear();
            Grid grid = pointPlacer.Grid;
            Vector3 gridForward = grid.transform.forward;
            
            // Foreach point in neighboring cells, build and memorize
            // a line which is orthogonal to the segment between that point and pointPosition, passing through their midPoint.
            foreach (Vector2Int coords in pointPlacer.EnumerateCellsInRadius(centerCellCoords))
            {
                if (coords == centerCellCoords)
                    continue;

                if (!points.TryGetValue(coords, out Vector3 position))
                    continue;

                Vector3 midPoint = (pointPosition + position) * 0.5F;
                Vector3 direction = (position - pointPosition).normalized;
                Vector3 normal = Vector3.Cross(direction, gridForward);
                var line = new Line(normal, midPoint);
                _lines.Add(line);
            }

            float maxSqrDistance = (grid.cellSize.sqrMagnitude + grid.cellGap.sqrMagnitude) * 2;

            HashSet<(int, int)> computedIndices = new();

            // Compute all the intersections between the lines.
            _intersectionPoints.Clear();
            for (int i = 0, count = _lines.Count; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (j == i) continue;

                    (int, int) indices = (Mathf.Min(i, j), Mathf.Max(i, j));
                    if (computedIndices.Contains(indices))
                        continue;
                    computedIndices.Add(indices);

                    if (_lines[i].TryGetIntersection(_lines[j], out Vector2 intersection))
                    {
                        if (MathUtility.SqrDistance((Vector2) pointPosition, intersection) > maxSqrDistance)
                            continue;
                        
                        _intersectionPoints.Add(new IntersectionData(i, j, intersection));
                    }
                }
            }

            // Filter the intersection points to keep only the ones that will form the inner polygon.
            for (int i = _intersectionPoints.Count - 1; i >= 0; i--)
            {
                bool remove = false;
                IntersectionData intersection = _intersectionPoints[i];
                var centerToIntersectionLine = Line.CreateFromPoints(pointPosition, intersection.position);

                Vector2 min = Vector2.Min(pointPosition, intersection.position);
                Vector2 max = Vector2.Max(pointPosition, intersection.position);

                for (int l = 0, count = _lines.Count; l < count; l++)
                {
                    if (l == intersection.lineIndexA || l == intersection.lineIndexB)
                        continue;

                    if (!centerToIntersectionLine.TryGetIntersection(_lines[l], out Vector2 intersectionToCenter))
                        continue;
                    
                    bool isOnSegment = intersectionToCenter.x >= min.x && intersectionToCenter.x <= max.x &&
                                             intersectionToCenter.y >= min.y && intersectionToCenter.y <= max.y;
                    if (!isOnSegment)
                        continue;
                    
                    remove = true;
                    break;
                }

                if (remove)
                {
                    _intersectionPoints.RemoveAt(i);
                }
            }
            
            // Sort intersection points according to their direction/angle so that we can
            // loop over the intersection points in clockwiser (or counter-clockwise) order.
            _intersectionPoints.Sort((idA, idB) =>
            {
                var zero = new Vector2(0, 0);
                float a = Vector2.Dot(idA.position, zero);
                float b = Vector2.Dot(idB.position, zero);
                return a.CompareTo(b);
            });
        }

        private readonly struct IntersectionData
        {
            public readonly int lineIndexA, lineIndexB;
            public readonly Vector2 position;

            public IntersectionData(int lineIndexA, int lineIndexB, Vector2 position)
            {
                this.lineIndexA = lineIndexA;
                this.lineIndexB = lineIndexB;
                this.position = position;
            }
        }

        private void OnDrawGizmos()
        {
            Vector2 cellSize = pointPlacer.Grid.cellSize + pointPlacer.Grid.cellGap;
            
            if (drawMidLines)
            {
                Gizmos.color = new Color(0.79f, 1f, 0.57f, 0.5f);
                for (int i = 0, count = _lines.Count; i < count; i++)
                {
                    _lines[i].GetSegment(
                        cellSize.x * -pointPlacer.extents.x,
                        cellSize.x * pointPlacer.extents.x,
                        out Vector2 start,
                        out Vector2 end);
                    Gizmos.DrawLine(start, end);
                }
            }
            
            if (drawInner || drawOuter)
            {
                Gizmos.color = new Color(0.24f, 0.5f, 1f);
                Vector3 centerPos = _currentCellPoint;
                for (int i = 0, count = _intersectionPoints.Count; i < count; i++)
                {
                    Vector2 iPosition = _intersectionPoints[i].position;
                    if (drawInner)
                    {
                        Gizmos.DrawLine(centerPos, iPosition);
                    }

                    if (i > 0 && drawOuter)
                    {
                        Gizmos.DrawLine(iPosition, _intersectionPoints[i - 1].position);
                    }
                }
            }
        }
    }
}