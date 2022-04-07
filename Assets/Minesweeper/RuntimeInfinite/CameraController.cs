using System;
using UnityEngine;

namespace Minesweeper.Runtime.Infinite
{
    [RequireComponent(typeof(Camera)), ExecuteAlways]
    public sealed class CameraController : MonoBehaviour, IWorldRectProvider
    {
        public Camera Cam => _cam != null ? _cam : _cam = GetComponent<Camera>();
        private Camera _cam;

        public Rect WorldRect => _worldRect;

        private Rect _worldRect;

        private void Update()
        {
            Cam.orthographic = true;
            
            if (Application.isPlaying)
            {
                ComputeWorldSpaceViewRect();
            }
        }

        private void ComputeWorldSpaceViewRect()
        {
            Transform camTransform = Cam.transform;
            Vector3 position = camTransform.position;
            float orthographicSize = Cam.orthographicSize;
            float w = orthographicSize / (1 / Cam.aspect * 0.5f) * 0.5f;
            _worldRect.yMax = position.y + orthographicSize;
            _worldRect.yMin = position.y - orthographicSize;
            _worldRect.xMin = position.x - w;
            _worldRect.xMax = position.x + w;
        }
    }
}