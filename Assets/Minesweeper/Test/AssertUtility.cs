using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Minesweeper.Tests
{
    public static class AssertUtility
    {
        public static void AreClose(float a, float b, float tolerance = 1E-13f)
        {
            Assert.IsTrue(Math.Abs(a - b) < tolerance, $"Expected {a} but got {b}.");
        }

        public static void AreClose(Vector2 a, Vector2 b, float tolerance = 1E-13f)
        {
            AreClose(a.x, b.x, tolerance);
            AreClose(a.y, b.y, tolerance);
        }
        
        public static void AreClose(Vector3 a, Vector3 b, float tolerance = 1E-13f)
        {
            AreClose(a.x, b.x, tolerance);
            AreClose(a.y, b.y, tolerance);
            AreClose(a.z, b.z, tolerance);
        }
    }
}