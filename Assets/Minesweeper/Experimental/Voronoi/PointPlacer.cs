using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minesweeper.Experimental.Voronoi
{
    [RequireComponent(typeof(Grid)), ExecuteAlways]
    public class PointPlacer : MonoBehaviour
    {
        public Vector2Int extents = new(1, 1);
        [Range(0, 1)] public float divergance = 1;

        public Transform _mockCursor;

        private Grid _grid;
        private readonly Dictionary<Vector3Int, Vector3> _currentPoints = new();

        private void OnEnable()
        {
            _grid ??= gameObject.GetComponent<Grid>();
            RefreshComputedPoints();
        }

        private void RefreshComputedPoints()
        {
            _currentPoints.Clear();
            ComputePoints(_currentPoints);
        }

        private void ComputePoints(IDictionary<Vector3Int, Vector3> points)
        {
            Vector3 halfSize = _grid.cellSize * 0.5f;

            PostProcessPointDelegate processPoint = divergance > float.Epsilon
                ? ApplyDivergance
                : delegate { };
            
            for (int x = -extents.x; x < extents.x; x++)
            {
                for (int y = -extents.y; y < extents.y; y++)
                {
                    Vector3Int index = new Vector3Int(x, y, 0);
                    Vector3 point = _grid.GetCellCenterWorld(index);
                    processPoint(ref point);
                    points.Add(index, point);
                }
            }

            void ApplyDivergance(ref Vector3 p)
            {
                var diverted = p + new Vector3(
                    x: Random.Range(-halfSize.x, halfSize.x),
                    y: Random.Range(-halfSize.y, halfSize.y),
                    z: Random.Range(-halfSize.z, halfSize.z));
                p = Vector3.Lerp(p, diverted, divergance);
            }
        }

        private delegate void PostProcessPointDelegate(ref Vector3 point);

        private void GetCellsInRadius(Vector3Int center, int radius, IList<Vector3Int> surrounding)
        {
            int xMin = center.x - radius;
            int xMax = center.x + radius;
            
            int yMin = center.y - radius;
            int yMax = center.y + radius;
            
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    surrounding.Add(new Vector3Int(x, y, center.z));
                }
            }
        }

#if UNITY_EDITOR

        private readonly List<Vector3Int> _debugCursorSurroundings = new ();

        private void OnDrawGizmos()
        {
            Vector3 gridCellSize = _grid.cellSize;
            Vector3 max = _grid.GetCellCenterWorld((Vector3Int)extents) - gridCellSize * 0.5f;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, max * 2);

            Vector3Int cursorCell;
            if (_mockCursor != null)
            {
                _debugCursorSurroundings.Clear();
                cursorCell = _grid.WorldToCell(_mockCursor.position);
                GetCellsInRadius(cursorCell, 2, _debugCursorSurroundings);
            }
            else
            {
                // a value that should never be present in the dictionary
                cursorCell = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            }

            Gizmos.color = Color.cyan;
            float pointRadius = Mathf.Min(gridCellSize.x, gridCellSize.y) * 0.1f;
            
            foreach (var (key, value) in _currentPoints)
            {
                var color = Gizmos.color;
                if (key == cursorCell || _debugCursorSurroundings.Contains(key))
                {
                    Gizmos.color = Color.magenta;
                }
                Gizmos.DrawWireSphere(value, pointRadius);
                Gizmos.color = color;
            }
        }

        [UnityEditor.MenuItem("CONTEXT/" + nameof(PointPlacer) + "/Refresh Computed Points")]
        private static void MenuItem_Refresh(UnityEditor.MenuCommand cmd)
        {
            if (cmd.context is PointPlacer pointPlacer)
            {
                pointPlacer.RefreshComputedPoints();
            }
        }

        private void OnValidate()
        {
            //RefreshComputedPoints();
        }
#endif

    }
}