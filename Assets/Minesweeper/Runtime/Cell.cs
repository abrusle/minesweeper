namespace Minesweeper.Runtime
{
    public struct Cell
    {
        public int value;
        public bool hasMine, hasFlag, isRevealed;

        /// <inheritdoc />
        public Cell(int value) : this()
        {
            this.value = value;
        }
    }
}