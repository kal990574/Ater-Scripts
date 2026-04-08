using System;
using _02.Scripts.Core.Timer.Domain;
using UnityEngine;

namespace _02.Scripts.Core.Timer.Manager
{
    public class ChapterTimerManager : MonoBehaviour, IChapterTimerManager
    {
        [Header("Timer")]
        [SerializeField] private float _timeLimitSeconds = 300f;

        [Header("Time Threshold")]
        [SerializeField] private float _warningThreshold = 180f;
        [SerializeField] private float _criticalThreshold = 60f;

        private ChapterTimerData _timerData;

        public float RemainingTime => _timerData?.RemainingTime ?? 0f;
        public TimerState CurrentState => _timerData?.State ?? TimerState.Ready;

        public event Action<TimerState> OnTimerStateChanged;
        public event Action OnTimerExpired;

        private void Start()
        {
            StartTimer();
        }

        public void StartTimer()
        {
            _timerData = new ChapterTimerData(_timeLimitSeconds);
            _timerData.State = TimerState.Running;
            OnTimerStateChanged?.Invoke(TimerState.Running);
        }

        public void StopTimer()
        {
            if (_timerData == null) return;

            _timerData.State = TimerState.Ready;
            OnTimerStateChanged?.Invoke(TimerState.Ready);
        }

        private void Update()
        {
            if (_timerData == null) return;
            if (_timerData.State == TimerState.Ready || _timerData.State == TimerState.Expired) return;

            _timerData.RemainingTime -= Time.deltaTime;

            if (_timerData.RemainingTime <= 0f)
            {
                _timerData.RemainingTime = 0f;
                SetState(TimerState.Expired);
                OnTimerExpired?.Invoke();
                return;
            }

            if (_timerData.RemainingTime <= _criticalThreshold)
                SetState(TimerState.Critical);
            else if (_timerData.RemainingTime <= _warningThreshold)
                SetState(TimerState.Warning);
        }

        private void SetState(TimerState newState)
        {
            if (_timerData.State == newState) return;

            _timerData.State = newState;
            OnTimerStateChanged?.Invoke(newState);
        }
    }
}