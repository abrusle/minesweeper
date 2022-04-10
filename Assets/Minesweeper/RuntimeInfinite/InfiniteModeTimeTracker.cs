using System;
using UnityEngine;
using UnityEngine.Events;

namespace Minesweeper.Runtime.Infinite
{
    public class InfiniteModeTimeTracker : MonoBehaviour
    {
        public float EndTime => _endTime;
        public float SecondsLeft => _endTime - Time.time;

        public bool IsRunning => _isRunning;
        
        [SerializeField] private TimeSettings timeSettings;
        [SerializeField] private UnityEvent onTimesUp;

        private float _endTime;
        private float _secondsLeftAtPause;
        private bool _isRunning;

        // TEMP
        private void Start()
        {
            StartTimer();
        }

        public void StartTimer()
        {
            StartTimer(timeSettings.BaseDuration);
        }
        
        private void StartTimer(float duration)
        {
            _endTime = Time.time + duration;
            _isRunning = true;
        }

        public void Boost()
        {
            _endTime += timeSettings.BoostDuration;
        }

        public void Pause()
        {
            if (!_isRunning) return;
            _isRunning = false;

            _secondsLeftAtPause = SecondsLeft;
        }

        public void Resume()
        {
            StartTimer(_secondsLeftAtPause);
        }

        private void Update()
        {
            if (!_isRunning)
            {
                return;
            }
            
            if (Time.time >= _endTime)
            {
                onTimesUp.Invoke();
                _isRunning = false;
            }
        }
    }
}