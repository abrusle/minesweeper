using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    [Flags]
    public enum CellStatusFlags : byte
    {
        None       = 0,
        IsRevealed = 0b001,
        HasMine    = 0b010,
        IsMarked   = 0b100
    }
    
    [DisallowMultipleComponent]
    public sealed class InfiniteLevelDataManager : MonoBehaviour
    {
        public event Action<Vector2Int, CellStatusFlags> CellStatusChanged;

        public BoundsInt Bounds => _generatedBounds;

        [SerializeField] private int seed;
        [SerializeField, Range(0, 1)] private float mineDensity = 0.65f;
        
        [NonSerialized] private readonly Dictionary<Vector2Int, CellStatusFlags> _generatedCells = new();
        [NonSerialized] private BoundsInt _generatedBounds = new()
        {
            min = new Vector3Int(int.MaxValue, int.MaxValue, 0),
            max = new Vector3Int(int.MinValue, int.MinValue, 0),
        };

        [PublicAPI] public bool IsCellGenerated(Vector2Int coords)
        {
            return _generatedCells.ContainsKey(coords);
        }

        [PublicAPI] public CellStatusFlags GetCellStatus(Vector2Int coords)
        {
            if (!_generatedCells.TryGetValue(coords, out CellStatusFlags cellStatus))
            {
                cellStatus = GenerateCellData(coords);
                _generatedCells[coords] = cellStatus;
                ContributeToBounds(coords.x, coords.y);
            }

            return cellStatus;
        }

        [PublicAPI] public void SetCellRevealed(Vector2Int coords)
        {
            var cellStatus = GetCellStatus(coords) | CellStatusFlags.IsRevealed;
            
            if (cellStatus.HasFlag(CellStatusFlags.IsMarked))
            {
                return;
            }
            
            _generatedCells[coords] = cellStatus;
            CellStatusChanged?.Invoke(coords, cellStatus);
        }

        [PublicAPI] public void SetCellMarked(Vector2Int coords, bool isMarked)
        {
            var cellStatus = GetCellStatus(coords);
            if (isMarked)
            {
                cellStatus |= CellStatusFlags.IsMarked;
            }
            else
            {
                cellStatus &= ~CellStatusFlags.IsMarked;
            }

            _generatedCells[coords] = cellStatus;
            CellStatusChanged?.Invoke(coords, cellStatus);
        }

        [PublicAPI] public void ToggleCellMarked(Vector2Int coords)
        {
            var cellStatus = GetCellStatus(coords);
            if (cellStatus.HasFlag(CellStatusFlags.IsMarked))
            {
                cellStatus &= ~CellStatusFlags.IsMarked;
            }
            else
            {
                cellStatus |= CellStatusFlags.IsMarked;
            }

            _generatedCells[coords] = cellStatus;
            CellStatusChanged?.Invoke(coords, cellStatus);
        }

        private CellStatusFlags GenerateCellData(Vector2Int coords)
        {
            float p = WhiteNoise.Sample(coords.x, coords.y, seed);
            return p < mineDensity ? CellStatusFlags.HasMine : CellStatusFlags.None;
        }

        private void ContributeToBounds(int x, int y)
        {
            if (x < _generatedBounds.xMin)
                _generatedBounds.xMin = x;
            
            if (x > _generatedBounds.xMax)
                _generatedBounds.xMax = x;
            
            if (y < _generatedBounds.yMin)
                _generatedBounds.yMin = y;

            if (y > _generatedBounds.yMax)
                _generatedBounds.yMax = y;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            seed = new System.Random().Next();
        }
#endif
    }
}