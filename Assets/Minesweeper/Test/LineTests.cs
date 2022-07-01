using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;

namespace Minesweeper.Tests
{
    using Runtime.Math;
    
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public static class LineTests
    {
        [Test]
        public static void IntersectionTest()
        {
            var lineA = new Line(2.5f, -12.3f);
            var lineB = new Line(-0.4f, 320.2f);
            var expectedIntersection = new Vector2(114.6551724137931f, 274.3379310344828f);
            const float tolerance = 1e-13f;

            bool hasIntersection = lineA.TryGetIntersection(in lineB, out Vector2 intersection, tolerance);
            
            Assert.IsTrue(hasIntersection);
            Assert.IsTrue(Math.Abs(intersection.x - expectedIntersection.x) < tolerance);
            Assert.IsTrue(Math.Abs(intersection.y - expectedIntersection.y) < tolerance);
        }

        [Test]
        public static void CreateLineFromPointsTestArbitrary()
        {
            Vector2 
                pointA = new (-0.6224586514781f, -8.8478204839917f),
                pointB = new (0.9338690981249f, 1.6729551033246f);
            var expected = new Line(6.76f, -4.64f);
            const float tolerance = 1e-5f;

            var line = Line.CreateFromPoints(pointA, pointB, tolerance);

            Debug.Log("Computed " + line);
            Debug.Log("Expected " + expected);
            Assert.IsTrue(Math.Abs(line.m - expected.m) < tolerance);
            Assert.IsTrue(Math.Abs(line.b - expected.b) < tolerance);
        }
        
        [Test]
        public static void CreateLineFromPointsTestHorizontal()
        {
            Vector2 
                pointA = new (-21.6224586514781f, 224.0736f),
                pointB = new (76.9338690981249f, 224.0736f);
            var expected = new Line(0, 224.0736f);
            const float tolerance = 1e-13f;

            var line = Line.CreateFromPoints(pointA, pointB, tolerance);

            Debug.Log("Computed " + line);
            Debug.Log("Expected " + expected);
            Assert.IsTrue(Math.Abs(line.m - expected.m) < tolerance);
            Assert.IsTrue(Math.Abs(line.b - expected.b) < tolerance);
        }
        
        [Test]
        public static void CreateLineFromPointsTestVertical()
        {
            Vector2 
                pointA = new (32, -8.8478204839917f),
                pointB = new (32, 1.6729551033246f);
            
            const float tolerance = 1e-13f;

            var line = Line.CreateFromPoints(pointA, pointB, tolerance);

            Debug.Log("Computed " + line);
            Assert.IsTrue(float.IsPositiveInfinity(line.m));
            Assert.IsTrue(float.IsInfinity(line.b));
        }

        [Test]
        public static void IsOnLineTest()
        {
            var line = new Line(2.5f, -12.3f);
            var pointThatIsOnTheLine = new Vector2(4.92f, 0);
            const float tolerance = 1E-13f;
            
            bool result = line.IsPointOnLine(pointThatIsOnTheLine, tolerance);
            Assert.IsTrue(result);
        }
        
        [Test]
        public static void IsAlmostOnLineTest()
        {
            var line = new Line(2.5f, -12.3f);
            var pointThatIsOnTheLine = new Vector2(4.923f, 0.01f);
            const float tolerance = 1E-13f;
            
            bool result = line.IsPointOnLine(pointThatIsOnTheLine, tolerance);
            Assert.IsFalse(result);
        }
        
        [Test]
        public static void IsNotOnLineTest()
        {
            var line = new Line(2.5f, -12.3f);
            var pointThatIsOnTheLine = new Vector2(-49.276f, 74.217f);
            const float tolerance = 1E-13f;
            
            bool result = line.IsPointOnLine(pointThatIsOnTheLine, tolerance);
            Assert.IsFalse(result);
        }
        
        [Test]
        public static void CreateFromTangentAndPointTest()
        {
            var tangent = new Vector2(  75.2988491326371f, 108.1214243955816f);
            var point =   new Vector2(-200.0006259982734f,  78.9606900388056f);
            const float tolerance = 1E-4f;

            var line = new Line(tangent, point, tolerance);

            var expected = new Line(1.4358974358974f, 366.141076087609f);
            
            Assert.IsTrue(Math.Abs(line.m - expected.m) < tolerance);
            Assert.IsTrue(Math.Abs(line.b - expected.b) < tolerance);
            Debug.Log("Computed " + line.ToString("F9"));
            Debug.Log("Expected " + expected.ToString("F9"));
        }
    }
}