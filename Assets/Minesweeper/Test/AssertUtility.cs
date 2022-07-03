using System;
using UnityEngine;

namespace Minesweeper.Tests
{
    public static class AssertUtility
    {
        public static bool AreClose(float a, float b, float tolerance = 1E-13f)
        {
            return Math.Abs(a - b) < tolerance;
        }

        public static bool AreClose(Vector2 a, Vector2 b, float tolerance = 1E-13f)
        {
            return AreClose(a.x, b.x, tolerance) &&
                   AreClose(a.y, b.y, tolerance);
        }
        
        public static bool AreClose(Vector3 a, Vector3 b, float tolerance = 1E-13f)
        {
            return AreClose(a.x, b.x, tolerance) &&
                   AreClose(a.y, b.y, tolerance) &&
                   AreClose(a.z, b.z, tolerance);
        }
    }
}