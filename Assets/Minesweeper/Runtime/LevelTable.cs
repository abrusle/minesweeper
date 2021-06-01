using System;
using System.Collections;
using System.Collections.Generic;
using Minesweeper.Runtime.Data;
using UnityEngine;

namespace Minesweeper.Runtime
{
    public class LevelTable : IEnumerable<LevelTable.TableCellEnumeration>
    {
        public bool Generated { get; private set; }
        public Vector2Int Size { get; private set; }
        public int CellCount => Size.x * Size.y;
        public int MineCount { get; private set; }
        
        private Cell[,] _cells;

        public Cell this[Vector2Int cellPos] => this[cellPos.x, cellPos.y];
        
        public Cell this[int x, int y]
        {
            get
            {
                CheckAccess(x, y);
                return _cells[x, y];
            }
        }

        public void Generate(LevelSettings settings, Vector2Int[] reservedEmpty)
        {
            _cells = LevelGenerator.GenerateNewLevel(
                settings.size.x,
                settings.size.y,
                settings.mineCount,
                reservedEmpty);

            MineCount = settings.mineCount;
            Size = settings.size;
            Generated = true;
        }

        public void MarkCellRevealed(Vector2Int cellPos)
        {
            CheckAccess(cellPos.x, cellPos.y);
            _cells[cellPos.x, cellPos.y].isRevealed = true;
        }

        public bool ToggleCellFlag(Vector2Int cellPos)
        {
            CheckAccess(cellPos.x, cellPos.y);
            bool desiredFlagState = !_cells[cellPos.x, cellPos.y].hasFlag;
            _cells[cellPos.x, cellPos.y].hasFlag = desiredFlagState;
            return desiredFlagState;
        }

        public void Clear()
        {
            _cells = null;
            MineCount = 0;
            Size = Vector2Int.zero;
            Generated = false;
        }

        private void CheckAccess(int x, int y)
        {
            if (_cells == null)
                throw new LevelTableException("Level not generated");
            
            if (!LevelUtility.IsCellWithinBounds(x, y, Size))
                throw new CellPositionOutOfRangeException(x, y);
        }

        public readonly struct TableCellEnumeration
        {
            public readonly Vector2Int position;
            public readonly Cell data;

            /// <inheritdoc />
            public TableCellEnumeration(int x, int y, Cell data) : this(new Vector2Int(x, y), data)
            {
            }

            public TableCellEnumeration(Vector2Int position, Cell data)
            {
                this.position = position;
                this.data = data;
            }
        }

        private struct TableEnumerator : IEnumerator<TableCellEnumeration>
        {
            private readonly Cell[,] _cells;
            private Vector2Int _currentIndex;
            private TableCellEnumeration _currentCell;
            
            private static Vector2Int BeforeStartIndex => new Vector2Int(-1, 0);

            public TableEnumerator(Cell[,] cells)
            {
                _cells = cells;
                _currentIndex = BeforeStartIndex;
                _currentCell = default;
            }

            /// <inheritdoc />
            bool IEnumerator.MoveNext()
            {
                if (++_currentIndex.x >= _cells.GetLength(0))
                {
                    if (++_currentIndex.y >= _cells.GetLength(1))
                    {
                        return false;
                    }
                    else
                    {
                        _currentIndex.x = 0;
                        _currentCell = CreateCurrentObject();
                    }
                }
                else
                {
                    _currentCell = CreateCurrentObject();
                }

                return true;
            }

            private TableCellEnumeration CreateCurrentObject()
            {
                return new TableCellEnumeration(_currentIndex, _cells[_currentIndex.x, _currentIndex.y]);
            }

            /// <inheritdoc />
            void IEnumerator.Reset()
            {
                _currentIndex = BeforeStartIndex;
            }

            /// <inheritdoc />
            TableCellEnumeration IEnumerator<TableCellEnumeration>.Current => _currentCell;

            /// <inheritdoc />
            object IEnumerator.Current => _currentCell;

            /// <inheritdoc />
            void IDisposable.Dispose() { }
        }

        private class CellPositionOutOfRangeException : LevelTableException
        {
            /// <inheritdoc />
            public CellPositionOutOfRangeException(int x, int y)
                : base("Cell coordinates outside of table range: " + $"({x}, {y})")
            {
            }
            
            /// <inheritdoc />
            public CellPositionOutOfRangeException(Vector2Int cell)
            : base("Cell coordinates outside of table range: " + cell)
            {
            }
        }
        
        private class LevelTableException : System.Exception
        {
            /// <inheritdoc />
            public LevelTableException()
            {
            }

            /// <inheritdoc />
            public LevelTableException(string message) : base(message)
            {
            }
        }

        /// <inheritdoc />
        IEnumerator<TableCellEnumeration> IEnumerable<TableCellEnumeration>.GetEnumerator()
        {
            return new TableEnumerator(_cells);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TableCellEnumeration>) this).GetEnumerator();
        }
    }
}