﻿using System;
using UnityEngine;

namespace Minesweeper.Runtime
{
    public class InputHandler : MonoBehaviour
    {
        public static event Action LeftClick, RightClick;
        
        private static InputHandler _Instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (_Instance != null) return;

            _Instance = new GameObject(nameof(InputHandler)).AddComponent<InputHandler>();
        }
        
        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
                return;
            }
            
            if (_Instance != this) Destroy(this);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                LeftClick?.Invoke();
            }

            if (Input.GetMouseButtonUp(1))
            {
                RightClick?.Invoke();
            }
        }
    }
}