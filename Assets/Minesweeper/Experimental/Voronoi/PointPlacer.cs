using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minesweeper.Runtime.Experimental.Voronoi
{
    [RequireComponent(typeof(Grid)), ExecuteAlways]
    public class PointPlacer : MonoBehaviour
    {
        public Vector2Int extents = new(1, 1);
        [Range(0, 1)] public float divergence = 1;

        private Grid _grid;
        private readonly Dictionary<Vector2Int, Vector3> _currentPoints = new();

        public Grid Grid => _grid;
        public IReadOnlyDictionary<Vector2Int, Vector3> CurrentPoints => _currentPoints;

        private readonly Dictionary<Vector2Int, int> _highlightedCoords = new();

        public void HighlightPoint(Vector2Int coords)
        {
            if (_highlightedCoords.ContainsKey(coords))
            {
                _highlightedCoords[coords]++;
            }
            else
            {
                _highlightedCoords[coords] = 1;
            }
        }

        public void UnhighlightPoint(Vector2Int coords)
        {
            if (!_highlightedCoords.ContainsKey(coords)) return;
            
            _highlightedCoords[coords]--;
            if (_highlightedCoords[coords] == 0)
            {
                _highlightedCoords.Remove(coords);
            }
        }
        
        public IEnumerable<Vector2Int> EnumerateCellsInRadius(Vector2Int center, int radius = 1)
        {
            int xMin = center.x - radius;
            int xMax = center.x + radius;
            
            int yMin = center.y - radius;
            int yMax = center.y + radius;
            
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                { 
                    var cellCoords = new Vector2Int(x, y);
                    if (_currentPoints.ContainsKey(cellCoords))
                    {
                        yield return cellCoords;
                    }
                }
            }
        }

        private void OnEnable()
        {
            _grid ??= gameObject.GetComponent<Grid>();
        }

        private void RefreshComputedPoints()
        {
            _currentPoints.Clear();
            ComputePoints(_currentPoints);
        }

        private void ComputePoints(IDictionary<Vector2Int, Vector3> points)
        {
            Vector3 halfSize = _grid.cellSize * 0.5f;

            PostProcessPointDelegate processPoint = divergence > float.Epsilon
                ? ApplyDivergence
                : delegate { };
            
            for (int x = -extents.x; x < extents.x; x++)
            {
                for (int y = -extents.y; y < extents.y; y++)
                {
                    Vector2Int index = new Vector2Int(x, y);
                    Vector3 point = _grid.GetCellCenterWorld((Vector3Int)index);
                    processPoint(ref point);
                    points.Add(index, point);
                }
            }

            void ApplyDivergence(ref Vector3 p)
            {
                var diverted = p + new Vector3(
                    x: Random.Range(-halfSize.x, halfSize.x),
                    y: Random.Range(-halfSize.y, halfSize.y),
                    z: 0/*Random.Range(-halfSize.z, halfSize.z)*/);
                p = Vector3.Lerp(p, diverted, divergence);
            }
        }

        private delegate void PostProcessPointDelegate(ref Vector3 point);

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Vector3 gridCellSize = _grid.cellSize;
            Vector3 max = _grid.GetCellCenterWorld((Vector3Int)extents) - gridCellSize * 0.5f;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, max * 2);

            float pointRadius = Mathf.Min(gridCellSize.x, gridCellSize.y) * 0.1f;
            
            foreach (var (coords, pointPosition) in _currentPoints)
            {
                Gizmos.color = _highlightedCoords.ContainsKey(coords) ? new Color(0.97f, 0.52f, 1f) : Color.cyan;
                Gizmos.DrawWireSphere(pointPosition, pointRadius);
            }
        }

        [UnityEditor.MenuItem("CONTEXT/" + nameof(PointPlacer) + "/Refresh Computed Points")]
        [SuppressMessage("ReSharper", "Unity.IncorrectMethodSignature")]
        private static void MenuItemContext_Refresh(UnityEditor.MenuCommand cmd)
        {
            if (cmd.context is PointPlacer pointPlacer)
            {
                pointPlacer.RefreshComputedPoints();
            }
        }

        [UnityEditor.MenuItem("Minesweeper/Voronoi/Debug/Refresh Computed Points", isValidateFunction:false)]
        private static void MenuItem_Refresh()
        {
            var pointPlacer = FindObjectOfType<PointPlacer>();
            if (pointPlacer == null)
            {
                Debug.Log("Not Point Placer found.");
            }
            else
            {
                pointPlacer.RefreshComputedPoints();
                Debug.Log("Refreshed Computed Points for " + pointPlacer, pointPlacer);
            }
        }
#endif

    }
}