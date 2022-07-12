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

        public BetterLine(float normalX, float normalY, float distance)
        {
            _normal = new Vector2(normalX, normalY);
            _normal.Normalize();
            this.distance = distance;
        }

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
            distance = Vector2.Dot(_normal, point);
        }

        public void Translate(in Vector2 translation)
        {
            distance += Vector2.Dot(_normal, translation);
        }

        public Vector2 Tangent
        {
            readonly get => new Vector2(_normal.y, -_normal.x);
            set
            {
                value.Normalize();
                _normal.x = -value.y;
                _normal.y = value.x;
            }
        }

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

        public readonly override string ToString()
        {
            return ToString("F3");
        }

        public readonly string ToString(string format)
        {
            return string.Format("(normal: [{0}, {1}], distance: {2})", 
                _normal.x.ToString(format), _normal.y.ToString(format), 
                distance.ToString(format));
        }

        public readonly bool TryGetSegment(in Vector2 min, in Vector2 max, out Vector2 start, out Vector2 end)
        {
            int failCount = 0;
            start = Vector2.positiveInfinity;
            end = Vector2.negativeInfinity;
            Vector2 intersection;

            var edge = new BetterLine(-1, 0, min.x); // left
            if (!TryIntersect(in this, ref start, Vector2.Min))
                goto fail;
            
            edge.distance = -max.x; // right
            if (!TryIntersect(in this, ref end, Vector2.Max))
                goto fail;
            
            (edge._normal.x, edge._normal.y) = (edge._normal.y, edge._normal.x);
            
            edge.distance = min.y; // bottom
            if (!TryIntersect(in this, ref start, Vector2.Min))
                goto fail;

            edge.distance = -max.y; // top
            if (!TryIntersect(in this, ref end, Vector2.Max))
                goto fail;

            bool TryIntersect(in BetterLine thisLine, ref Vector2 startOrEnd, Func<Vector2, Vector2, Vector2> minOrMax)
            {
                if (!TryGetIntersection(thisLine, edge, out intersection))
                {
                    failCount++;
                    if (failCount > 2)
                        return false;
                }
                else startOrEnd = minOrMax(startOrEnd, intersection);

                return true;
            }
            
            return true;
            
            fail:
            {
                start = end = default;
                return false;
            }
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
            // TODO fix this...
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
            var tangent = pointB - pointA;
            return new BetterLine(new Vector2(-tangent.y, tangent.x), pointA);
        }
    }
}