using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;
using UnityAssert = UnityEngine.Assertions.Assert;
// ReSharper disable InconsistentNaming

namespace Minesweeper.Tests
{
    using Runtime.Math;
    
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public static class BetterLineTests
    {
        [Test]
        public static void IntersectionTest()
        {
            var lineA = new BetterLine(0.6f, 0.8f, 1.8f);;
            var lineB = new BetterLine(-0.3969111506855f, 0.9178570359601f, 2.6295363732912f);
            var expectedIntersection = new Vector2(-0.52f, 2.64f);
            const float tolerance = 1e-13f;

            bool hasIntersection = BetterLine.TryGetIntersection(in lineA, in lineB, out Vector2 intersection, tolerance);
            
            Assert.IsTrue(hasIntersection);
            Debug.Log("Computed Intersection: " + intersection);
            Debug.Log("Expected Intersection: " + expectedIntersection);
            AssertUtility.AreClose(intersection, expectedIntersection, tolerance);
        }

        [Test]
        public static void CreateLineFromNormalAndPointTestArbitrary()
        {
            var line = new BetterLine(new Vector2(0.6f, 0.8f), new Vector2(3, 0));
            var expectedNormal = new Vector2(0.6f, 0.8f);
            const float expectedDistance = 1.8f;
            const float tolerance = 1e-5f;
            
            Debug.Log("Computed " + line);
            Debug.Log("Expected: normal " + expectedNormal + ", distance: " + expectedDistance);
            AssertUtility.AreClose(line.Normal, expectedNormal, tolerance);
            AssertUtility.AreClose(line.distance, expectedDistance, tolerance);
        }

        [Test]
        public static void CreateLineFromArbitraryPointsTest()
        {
            Vector2 
                pointA = new (-1, 3),
                pointB = new (3, 0);
            var expectedNormal = new Vector2(0.6f, 0.8f);
            const float expectedDistance = 1.8f;
            const float tolerance = 1e-5f;

            var line = BetterLine.CreateFromPoints(pointA, pointB);

            Debug.Log("Computed " + line);
            Debug.Log("Expected: normal " + expectedNormal + ", distance: " + expectedDistance);
            AssertUtility.AreClose(line.Normal, expectedNormal, tolerance);
            AssertUtility.AreClose(line.distance, expectedDistance, tolerance);
        }

        [Test]
        public static void CreateLineFromPointsTestHorizontal()
        {
            Vector2 
                pointA = new (-3, 3),
                pointB = new (1, 3);
            var expected = new BetterLine(new Vector2(0, 1), 3);
            const float tolerance = 1e-13f;

            var line = BetterLine.CreateFromPoints(pointA, pointB);

            Debug.Log("Computed " + line);
            Debug.Log("Expected " + expected);
            AssertUtility.AreClose(line.Normal, expected.Normal, tolerance);
            AssertUtility.AreClose(line.distance, expected.distance, tolerance);
        }
        
        [Test]
        public static void CreateLineFromPointsTestVertical()
        {
            Vector2 
                pointA = new (-3, -8.8478204839917f),
                pointB = new (-3, 1.6729551033246f);
            var expected = new BetterLine(new Vector2(-1, 0), 3);
            const float tolerance = 1e-13f;

            var line = BetterLine.CreateFromPoints(pointA, pointB);

            Debug.Log("Computed " + line);
            Debug.Log("Expected " + expected);
            AssertUtility.AreClose(line.Normal, expected.Normal, tolerance);
            AssertUtility.AreClose(line.distance, expected.distance, tolerance);
        }

        [Test]
        public static void LineRectIntersectionTest()
        {
            var line = new BetterLine(new Vector2(0.6f, 0.8f), new Vector2(2.4f, 1.8f));
            var min = new Vector2(-3, 1);
            var max = new Vector2(0.5f, 3.5f);

            var expectedIntersectionA = new Vector2(0.5f, 3.225f);
            var expectedIntersectionB = new Vector2(0.133333333333333333333333333333333333333333333333333333333333333333f, 3.5f);

            bool hasIntersection = line.TryGetSegment(in min, in max, out Vector2 intersectA, out Vector2 intersectB);
            
            UnityAssert.IsTrue(hasIntersection);
            AssertUtility.AreClose(intersectA, expectedIntersectionA);
            AssertUtility.AreClose(intersectB, expectedIntersectionB);
        }
    }
}