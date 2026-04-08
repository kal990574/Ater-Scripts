using System;
using _02.Scripts.Core.Timer.Domain;

namespace _02.Scripts.Core.Timer.Manager
{
    public interface IChapterTimerManager
    {
        float RemainingTime { get; }
        TimerState CurrentState { get; }

        event Action<TimerState> OnTimerStateChanged;
        event Action OnTimerExpired;

        void StartTimer(float totalSeconds);
        void StopTimer();
        void Tick(float deltaTime);
    }
}