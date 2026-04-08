using _02.Scripts.Core;
using _02.Scripts.Core.Timer.Domain;
using _02.Scripts.Core.Timer.Manager;
using TMPro;
using UnityEngine;

namespace _02.Scripts.Core.Timer.Component
{
    public class ChapterTimerDisplay : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text _timerText;

        [Header("색상 설정")]
        [SerializeField] private Color _normalColor = new Color(1f, 1f, 1f);
        [SerializeField] private Color _warningColor = new Color(1f, 0.6f, 0f);
        [SerializeField] private Color _criticalColor = Color.red;

        [Header("깜빡임 설정")]
        [SerializeField] private float _criticalBlinkSpeed = 3f;

        private IChapterTimerManager _timerManager;
        private int _lastDisplayedSecond = -1;

        private void Start()
        {
            _timerManager = Managers.Get<IChapterTimerManager>();
            _timerManager.OnTimerStateChanged += OnTimerStateChanged;
        }

        private void OnDestroy()
        {
            if (_timerManager != null)
                _timerManager.OnTimerStateChanged -= OnTimerStateChanged;
        }

        private void Update()
        {
            if (_timerManager == null) return;
            if (_timerManager.CurrentState == TimerState.Ready) return;

            _timerManager.Tick(Time.deltaTime);
            UpdateDisplay();
            UpdateBlink();
        }

        private void UpdateDisplay()
        {
            float remaining = _timerManager.RemainingTime;
            int totalSeconds = (int)remaining;

            if (totalSeconds == _lastDisplayedSecond) return;
            _lastDisplayedSecond = totalSeconds;

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            _timerText.SetText("{0:00}:{1:00}", minutes, seconds);
        }

        private void UpdateBlink()
        {
            if (_timerManager.CurrentState == TimerState.Critical)
            {
                float alpha = (Mathf.Sin(Time.unscaledTime * _criticalBlinkSpeed) + 1f) * 0.5f;
                _timerText.alpha = alpha;
            }
        }

        private void OnTimerStateChanged(TimerState state)
        {
            _lastDisplayedSecond = -1;

            switch (state)
            {
                case TimerState.Running:
                    _timerText.color = _normalColor;
                    _timerText.alpha = 1f;
                    break;
                case TimerState.Warning:
                    _timerText.color = _warningColor;
                    _timerText.alpha = 1f;
                    break;
                case TimerState.Critical:
                    _timerText.color = _criticalColor;
                    break;
                case TimerState.Expired:
                    _timerText.color = _criticalColor;
                    _timerText.SetText("00:00");
                    break;
            }
        }
    }
}