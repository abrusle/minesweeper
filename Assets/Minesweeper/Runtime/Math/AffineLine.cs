using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Minesweeper.Runtime.Math
{
    using Maths = System.Math;
    
    [Serializable]
    public struct AffineLine
    {
        private const float DefaultTolerance = 1E-13f;
        
        public float m, b;

        public readonly Vector2 Tangent
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Vector2(1, m);
        }

        public readonly Vector2 Normal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Vector2(-m, 1);
        }

        public AffineLine(float m, float b)
        {
            this.m = m;
            this.b = b;
        }

        public AffineLine(in Vector2 tangent, in Vector2 point, float tolerance = DefaultTolerance)
        {
            m = Maths.Abs(tangent.x) < tolerance ? 0 : tangent.y / tangent.x;
            b = point.y - m * point.x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float GetY(float x)
        {
            return m * x + b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector2 GetPoint(float x) => new Vector2(x, GetY(x));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void GetSegment(float xMin, float xMax, out Vector2 start, out Vector2 end)
        {
            start = GetPoint(xMin);
            end = GetPoint(xMax);
        }

        public readonly bool TryGetIntersection(in AffineLine other, out Vector2 intersection, float tolerance = DefaultTolerance)
        {
            float slopeDiff = m - other.m;
            if (Maths.Abs(slopeDiff) < tolerance)
            {
                intersection = default;
                return false;
            }

            intersection.x = (other.b - b) / slopeDiff;
            intersection.y = GetY(intersection.x);

            return true;
        }

        public readonly bool IsPointOnLine(in Vector2 point, float tolerance = DefaultTolerance)
        {
            return Maths.Abs(GetY(point.x) - point.y) < tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsPointAboveLine(in Vector2 point)
        {
            return GetY(point.x) < point.y;
        }

        public readonly bool Approximately(in AffineLine other)
        {
            return Mathf.Approximately(m, other.m) && Mathf.Approximately(b, other.b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly AffineLine GetOrthogonalLine(float intersectionX)
        {
            return new AffineLine(Normal, GetPoint(intersectionX));
        }

        public readonly override string ToString()
        {
            return ToString("F2");
        }
        public readonly string ToString(string format)
        {
            return string.Format("Line (y = {0}x {1} {2})", m.ToString(format), b < 0 ? "" : "+", b.ToString(format));
        }

        public static AffineLine CreateFromPoints(Vector2 pointA, Vector2 pointB, float tolerance = DefaultTolerance)
        {
            Vector2 diff = pointB - pointA;
            float m = Maths.Abs(diff.x) < tolerance ? float.PositiveInfinity : diff.y / diff.x;
            float b = pointA.y - m * pointA.x;
            return new AffineLine(m, b);
        }
    }
}