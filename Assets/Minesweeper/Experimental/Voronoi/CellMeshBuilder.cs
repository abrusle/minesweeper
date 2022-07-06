using System.Collections.Generic;
using Minesweeper.Runtime.Math;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.Rendering;

namespace Minesweeper.Runtime.Experimental.Voronoi
{
    [ExecuteAlways]
    public class CellMeshBuilder : MonoBehaviour
    {
        public PointPlacer pointPlacer;
        public MeshFilter targetMeshFilter;

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

        private void OnEnable()
        {
            if (pointPlacer != null)
            {
                pointPlacer.OnPointsGenerated += OnPointsChanged;
            }
        }

        private void OnDisable()
        {
            if (pointPlacer != null)
                pointPlacer.OnPointsGenerated -= OnPointsChanged;
        }

        private void OnPointsChanged()
        {
            _meshRebuildNeeded = true;
        }

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
                if (targetMeshFilter != null)
                {
                    var mesh = targetMeshFilter.sharedMesh;
                    if (mesh == null)
                    {
                        mesh = new Mesh();
                        mesh.name = "Cell " + _currentCellCoords;
                        targetMeshFilter.sharedMesh = mesh;
                    }

                    FillMesh(mesh, targetMeshFilter.transform.worldToLocalMatrix);
                }
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
            
            // TODO : add lines corresponding to the edges/bounds of pointPlacer.
            
            // Foreach point in neighboring cells, build and memorize
            // a line which is orthogonal to the segment between that point and pointPosition, passing through their midPoint.
            foreach (Vector2Int coords in pointPlacer.EnumerateCellsInRadius(centerCellCoords, 2))
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
                        float angle = Vector2.SignedAngle(intersection - (Vector2)pointPosition, Vector2.right);
                        _intersectionPoints.Add(new IntersectionData(i, j, intersection, angle));
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
            _intersectionPoints.Sort((idA, idB) => idA.angle.CompareTo(idB.angle));
        }

        private readonly struct IntersectionData
        {
            public readonly int lineIndexA, lineIndexB;
            public readonly Vector2 position;
            public readonly float angle;

            public IntersectionData(int lineIndexA, int lineIndexB, Vector2 position, float angle)
            {
                this.lineIndexA = lineIndexA;
                this.lineIndexB = lineIndexB;
                this.position = position;
                this.angle = angle;
            }
        }

        private void FillMesh(Mesh mesh, Matrix4x4 worldToLocalMatrix)
        {
            int count = _intersectionPoints.Count;
            if (count < 2)
                return;

            var vertices = ListPool<Vector3>.Get();
            var tris = ListPool<int>.Get();

            for (int i = 0; i < count; i++)
            {
                Vector2 p = worldToLocalMatrix.MultiplyPoint(_intersectionPoints[i].position);
                vertices.Add(p);

                if (i < 2)
                    continue;

                tris.Add(0);
                tris.Add(i - 1);
                tris.Add(i);
            }

            if (vertices.Count != mesh.vertexCount)
            {
                mesh.Clear();
            }

            mesh.SetVertices(vertices, 0, count, MeshUpdateFlags.Default);
            mesh.SetTriangles(tris, 0, true, 0);

            ListPool<Vector3>.Release(vertices);
            ListPool<int>.Release(tris);
        }

        private void OnDrawGizmos()
        {
            Vector2 cellSize = pointPlacer.Grid.cellSize + pointPlacer.Grid.cellGap;
            
            if (drawMidLines)
            {
                Gizmos.color = new Color(0.79f, 1f, 0.57f, 0.25f);
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
                    IntersectionData intersection = _intersectionPoints[i];
                    Vector2 iPosition = intersection.position;
                    if (drawInner)
                    {
                        Gizmos.DrawLine(centerPos, iPosition);
                    }

                    UnityEditor.Handles.color = Gizmos.color;
                    UnityEditor.Handles.Label(iPosition, $"{intersection.angle:F3}°");
                    
                    if (drawOuter)
                    {
                        Gizmos.DrawLine(iPosition, _intersectionPoints[(i == 0 ? count : i) - 1].position);
                    }
                }
            }
        }
    }
}