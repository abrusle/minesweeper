using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Minesweeper.Runtime.Utility;
using Minesweeper.Runtime.Views;
using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    [RequireComponent(typeof(Grid))]
    public sealed partial class InfiniteGridGenerator : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private Transform cellPrefab;
        
        [Header("References")]
        [SerializeField] private Grid grid;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private InfiniteLevelDataManager levelDataManager;

        [Header("Settings")]
        [SerializeField, Min(0)] private float cleanIntervalDuration = 5;

        private const int PoolCapacity = 2048, DictionaryCapacity = 4096;
        
        private readonly Stack<Transform> _cellPool = new(PoolCapacity);
        private readonly List<ICellInitializer> _cellPopulators = new();
        private readonly Dictionary<Vector3Int, Transform> _cellsInView = new(DictionaryCapacity);

        private Coroutine _cleanUpCoroutine;
        private Vector3Int _currentMinCoord, _currentMaxCoord;
        private WaitForSecondsRealtime _cleanInterval;
        private bool _needsCleanup = true;
        private Rect _viewRect = new(-5, -5, 10, 10);
        
        private void OnEnable()
        {
            levelDataManager.CellStatusChanged += OnCellStatusChanged;
            _cleanUpCoroutine = StartCoroutine(CleanUpLoop());
        }

        private void OnDisable()
        {
            if (_cleanUpCoroutine != null)
            {
                StopCoroutine(_cleanUpCoroutine);
                _cleanUpCoroutine = null;
            }
            
            _needsCleanup = true;
        }

        private void Update()
        {
            GetComponents(_cellPopulators);
            var viewRect = _viewRect = cameraController.WorldRect;
            Vector3Int min = grid.WorldToCell(viewRect.min);
            Vector3Int max = grid.WorldToCell(viewRect.max);

            if (min == _currentMinCoord && max == _currentMaxCoord)
            {
                return;
            }

            _currentMinCoord = min;
            _currentMaxCoord = max;
            _needsCleanup = true;

            for (int x = _currentMinCoord.x; x <= _currentMaxCoord.x; x++)
            {
                for (int y = _currentMinCoord.y; y <= _currentMaxCoord.y; y++)
                {
                    Vector3Int coord = new(x, y, 0);
                    if (!_cellsInView.TryGetValue(coord, out Transform cellInstance))
                    {
                        cellInstance = GetCellInstance();
                        cellInstance.position = grid.GetCellCenterWorld(coord);
                        _cellsInView[coord] = cellInstance;
                        cellInstance.gameObject.SetActive(true);
                        cellInstance.gameObject.name = $"Cell ({x}, {y})";
                        OnCellBecameVisible((Vector2Int)coord, cellInstance);
                    }
                }
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CleanUpLoop()
        {
            var cleanupList = new List<Vector3Int>();
            _cleanInterval = new WaitForSecondsRealtime(cleanIntervalDuration);
            while (true)
            {
                if (_needsCleanup)
                {
                    CleanCells();
                    _needsCleanup = false;
                }
                yield return _cleanInterval;
            }

            _cleanUpCoroutine = null;

            void CleanCells()
            {
                foreach (Vector3Int coord in _cellsInView.Keys)
                {
                    if (coord.x < _currentMinCoord.x ||
                        coord.y < _currentMinCoord.y ||
                        coord.x > _currentMaxCoord.x ||
                        coord.y > _currentMaxCoord.y)
                    {
                        cleanupList.Add(coord);
                    }
                }

                int cellCount = cleanupList.Count;
                for (int i = 0; i < cellCount; i++)
                {
                    _cellsInView.Remove(cleanupList[i], out Transform cellInstance);
                    ReleaseCellInstance(cellInstance);
                }
                
                cleanupList.Clear();
            }
        }

        private void ReleaseCellInstance(Transform cellInstance)
        {
            if (_cellPool.Count == PoolCapacity)
            {
                Destroy(cellInstance.gameObject);
            }
            else
            {
                cellInstance.gameObject.SetActive(false);
                _cellPool.Push(cellInstance);
            }
        }

        private Transform GetCellInstance()
        {
            if (!_cellPool.TryPop(out Transform cell))
            {
                cell = Instantiate(cellPrefab, transform);
            }

            return cell;
        }

        private void OnCellStatusChanged(Vector2Int coord, CellStatusFlags cellStatus)
        {
            if (coord.x < _currentMinCoord.x ||
                coord.y < _currentMinCoord.y ||
                coord.x > _currentMaxCoord.x ||
                coord.y > _currentMaxCoord.y)
            {
                return;
            }

            Transform cellInstance = _cellsInView[(Vector3Int)coord];
            UpdateCellViews(cellInstance, coord, cellStatus);
        }

        private void OnCellBecameVisible(Vector2Int cellPosition, Transform cellInstance)
        {
            var cellStatus = levelDataManager.IsCellGenerated(cellPosition) ? levelDataManager.GetCellStatus(cellPosition) : CellStatusFlags.None;
            UpdateCellViews(cellInstance, cellPosition, cellStatus);
        }

        private void UpdateCellViews(Transform cellInstance, Vector2Int cellPosition, CellStatusFlags cellStatus)
        {
            var cellview = cellInstance.GetComponent<CellView>();
            if (cellview == null) return;

            bool isRevealed = cellStatus.HasFlag(CellStatusFlags.IsRevealed);
            bool isMarked = !isRevealed && cellStatus.HasFlag(CellStatusFlags.IsMarked);
            cellview.textMesh.enabled = isRevealed;
            cellview.ToggleFlag(isMarked);
            if (isRevealed)
            {
                if (cellStatus.HasFlag(CellStatusFlags.HasMine))
                {
                    cellview.textMesh.text = "*";
                }
                else
                {
                    const int neighborCount = 8;
                    var neighbors = ArrayPool<Vector2Int>.Get(neighborCount);
                    LevelUtility.GetAdjacentCellsSquare(cellPosition, neighbors);
                    int cellValue = 0;
                    for (int i = 0; i < neighborCount; ++i)
                    {
                        var neighborStatus = levelDataManager.GetCellStatus(neighbors[i]);
                        if (neighborStatus.HasFlag(CellStatusFlags.HasMine))
                        {
                            cellValue++;
                        }
                    }

                    ArrayPool<Vector2Int>.Release(neighbors);

                    cellview.textMesh.text = cellValue.ToString();
                }
            }
        }

        #if UNITY_EDITOR

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                _cleanInterval = new WaitForSecondsRealtime(cleanIntervalDuration);
            }
        }

        private void Reset()
        {
            grid = GetComponent<Grid>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            GizmoHelper.DrawRect(_viewRect);
        }
        #endif
    }
}