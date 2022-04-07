using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    public interface IWorldRectProvider
    {
        Rect WorldRect { get; }
    }
}