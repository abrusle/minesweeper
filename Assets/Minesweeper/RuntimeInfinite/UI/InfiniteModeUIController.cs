using System;
using Minesweeper.Runtime.UI.Views;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minesweeper.Runtime.Infinite.UI
{
    public sealed class InfiniteModeUIController : UIBehaviour
    {
        [Header("External References")]
        [SerializeField] private InfiniteModeTimeTracker timeTracker;

        [Header("View References")]
        [SerializeField] private GaugeView timeGauge;
        [SerializeField] private TextMeshProUGUI timeText;

        [Header("Settings")]
        [SerializeField] private TimeSettings timeSettings;

        private void Update()
        {
            if (timeTracker.IsRunning)
            {
                float t = Mathf.Max(0, timeTracker.SecondsLeft);
                timeText.text = t < 11 ? t.ToString("N1") : Mathf.Round(t).ToString("N0") + " s";
                timeGauge.Level = Mathf.InverseLerp(0, timeSettings.BaseDuration, t);
            }
        }
    }
}