using System;
using _02.Scripts.Core.Timer.Domain;
using UnityEngine;

namespace _02.Scripts.Core.Timer.Manager
{
    public class ChapterTimerManager : IChapterTimerManager
    {
        [Header("time threshold")]
        [SerializeField] private float _warningThreshold = 180f;
        [SerializeField] private float _criticalThreshold = 60f;

        private ChapterTimerData _timerData;

        public float RemainingTime => _timerData?.RemainingTime ?? 0f;
        public TimerState CurrentState => _timerData?.State ?? TimerState.Ready;

        public event Action<TimerState> OnTimerStateChanged;
        public event Action OnTimerExpired;

        public void StartTimer(float totalSeconds)
        {
            _timerData = new ChapterTimerData(totalSeconds);
            _timerData.State = TimerState.Running;
            OnTimerStateChanged?.Invoke(TimerState.Running);
        }

        public void StopTimer()
        {
            if (_timerData == null) return;

            _timerData.State = TimerState.Ready;
            OnTimerStateChanged?.Invoke(TimerState.Ready);
        }

        public void Tick(float deltaTime)
        {
            if (_timerData == null) return;
            if (_timerData.State == TimerState.Ready || _timerData.State == TimerState.Expired) return;

            _timerData.RemainingTime -= deltaTime;

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