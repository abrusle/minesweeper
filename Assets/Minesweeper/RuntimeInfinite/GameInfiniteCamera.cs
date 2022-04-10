using System;
using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    [RequireComponent(typeof(Camera)), ExecuteAlways]
    public sealed class GameInfiniteCamera : MonoBehaviour, IWorldRectProvider
    {
        public Camera Cam => _cam != null ? _cam : _cam = GetComponent<Camera>();
        private Camera _cam;

        public Rect WorldRect => _worldRect;

        [SerializeField, Min(0.01f)] private float sizeMin = 2;
        [SerializeField, Min(0.01f)] private float sizeMax = 10;
        [SerializeField] private float _transitionTime = 0.25f;

        [NonSerialized] private float? _targetSize;
        [NonSerialized] private float _sizeVelocity;
        [NonSerialized] private Vector3? _targetPosition;
        [NonSerialized] private Vector3 _positionVelocity;
        [NonSerialized] private Rect _worldRect;

        private void Update()
        {
            Cam.orthographic = true;

            if (!Application.isPlaying)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            if (_targetSize.HasValue)
            {
                float targetSize = _targetSize.Value;
                float newSize = Mathf.SmoothDamp(Cam.orthographicSize, targetSize, ref _sizeVelocity, _transitionTime, float.PositiveInfinity, deltaTime);
                Cam.orthographicSize = newSize;

                if (Mathf.Approximately(targetSize, newSize))
                {
                    _targetSize = null;
                }
            }

            if (_targetPosition.HasValue)
            {
                Vector3 targetPosition = _targetPosition.Value;
                var newPos = Vector3.SmoothDamp(transform.position, targetPosition, ref _positionVelocity, _transitionTime, float.PositiveInfinity, deltaTime);
                transform.position = newPos;
                
                if (newPos.Approximately(targetPosition))
                {
                    _targetPosition = null;
                }
            }
            
            ComputeWorldSpaceViewRect();
        }

        public void FrameArea(Rect worldRect)
        {
            var (orthographicSize, position) = WorldRectToPositionAndSize(worldRect);
            _targetSize = Mathf.Clamp(orthographicSize, sizeMin, sizeMax);
            position.z = transform.position.z;
            _targetPosition = position;
        }

        private void ComputeWorldSpaceViewRect()
        {
            Transform camTransform = Cam.transform;
            Vector3 position = camTransform.position;
            float orthographicSize = Cam.orthographicSize;

            _worldRect = PositionAndSizeToWorldRect(position, orthographicSize, 1 / Cam.aspect);
        }

        private static (float orthographicSize, Vector3 position) WorldRectToPositionAndSize(Rect worldRect)
        {
            return (worldRect.height * 0.5f, worldRect.center);
        }

        private static Rect PositionAndSizeToWorldRect(Vector3 worldPosition, float orthographicSize, float aspect)
        {
            float w = orthographicSize / (aspect * 0.5f) * 0.5f;
            return new Rect
            {
                yMax = worldPosition.y + orthographicSize,
                yMin = worldPosition.y - orthographicSize,
                xMin = worldPosition.x - w,
                xMax = worldPosition.x + w,
            };
        }
    }
}