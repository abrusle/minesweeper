using System;
using UnityEngine;

namespace Minesweeper.Runtime.Math
{
    [Serializable]
    public struct BetterLine
    {
        /// <summary>
        /// Normal vector of the line
        /// </summary>
        private Vector2 _normal;

        /// <summary>
        /// Normal vector of the line
        /// </summary>
        public Vector2 Normal
        {
            readonly get => _normal;
            set
            {
                value.Normalize();
                _normal = value;
            }
        }

        /// <summary>
        /// The distance measured from the line to the origin, along the line's normal.
        /// </summary>
        public float distance;

        public BetterLine(Vector2 normal, float distance)
        {
            normal.Normalize();
            _normal = normal;
            this.distance = distance;
        }

        public BetterLine(Vector2 normal, in Vector2 point)
        {
            normal.Normalize();
            _normal = normal;
            distance = -Vector2.Dot(_normal, point);
        }

        public void Translate(in Vector2 translation)
        {
            distance += Vector2.Dot(_normal, translation);
        }

        public readonly Vector2 Tangent => new Vector2(_normal.y, -_normal.x);

        /// <summary>
        /// For a given point returns the closest point on the line.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public readonly Vector2 ClosestPointOnLine(in Vector2 point)
        {
            float num = Vector2.Dot(_normal, point) + distance;
            return point - _normal * num;
        }

        /// <summary>
        /// Returns a signed distance from this line to the point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public readonly float GetDistanceToPoint(in Vector2 point)
        {
            return Vector2.Dot(_normal, point) + distance;
        }

        public readonly void GetSegment(in Vector2 min, in Vector2 max, out Vector2 start, out Vector2 end)
        {
            // TODO
            start = default;
            end = default;
        }

        public readonly bool Raycast(Ray2D ray, out float enter, float tolerance = 1E-13f)
        {
            float a = Vector2.Dot(ray.direction, _normal);
            if (Mathf.Abs(a) <= tolerance)
            {
                enter = 0.0f;
                return false;
            }

            float num = -Vector2.Dot(ray.origin, _normal) - distance;
            enter = num / a;
            return enter > 0.0f;
        }

        public readonly bool TryGetIntersection(in BetterLine other, out Vector2 intersection, float tolerance = 1E-13f)
        {
            return TryGetIntersection(in this, in other, out intersection, tolerance);
        }

        public static bool TryGetIntersection(in BetterLine lineA, in BetterLine lineB, out Vector2 intersection, float tolerance = 1E-13f)
        {
            Vector2 direction = lineA.Tangent;
            float a = Vector2.Dot(direction, lineB._normal);
            if (Mathf.Abs(a) <= tolerance)
            {
                intersection = new Vector2(float.NaN, float.NaN);
                return false;
            }

            Vector2 origin = lineA._normal * lineA.distance;
            float num = -Vector2.Dot(origin, lineB._normal) - lineB.distance;
            intersection = origin + direction * (num / a);
            return true;
        }

        public static BetterLine CreateFromPoints(in Vector2 pointA, in Vector2 pointB)
        {
            var normal = pointB.magnitude * pointA + pointA.magnitude * pointB;
            return new BetterLine(normal, pointA);
        }
    }
}