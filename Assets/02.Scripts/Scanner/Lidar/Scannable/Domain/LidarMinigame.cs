using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LidarMinigame
{
//     private readonly LidarProgressSetting _settings;
//     private readonly IQuickTimeEvent _quickTimeEvent;
//     private readonly LidarProgress _progress;
//
//     private float _nextMinigameTriggerProgress;
//
//     public LidarMinigame(
//         LidarProgressSetting settings,
//         IQuickTimeEvent quickTimeEvent,
//         LidarProgress progress )
//     {
//         _settings = settings;
//         _quickTimeEvent = quickTimeEvent;
//         _progress = progress;
//
//         ResetNextTrigger();
//     }
//     
//     
//     //미니게임 진입 가능 검사 => 가능하면 미니게임 실행
//     public bool ShouldEnterMinigame(ELidarTargetState currentState)
//     {
//         if (_quickTimeEvent == null)
//         {
//             return false;
//         }
//
//         if (currentState != ELidarTargetState.OnProgress)
//         {
//             return false;
//         }
//
//         if (_progress.ProgressSinceLastMinigame < _nextMinigameTriggerProgress)
//         {
//             return false;
//         }
//
//         _progress.ConsumeMinigameProgress();
//         ResetNextTrigger();
//         return true;
//     }
//
//     public void BeginOrReturnToProgress()
//     {
//         if (_quickTimeEvent == null)
//         {
//             return;
//         }
//
//         _quickTimeEvent.Begin();
//     }
//
//     public EQuickTimeEventResult? Tick(float deltaTime)
//     {
//         if (_quickTimeEvent == null)
//         {
//             return null;
//         }
//
//         _quickTimeEvent.Tick(deltaTime);
//
//         if (_quickTimeEvent.IsFinished == true)
//         {
//             return _quickTimeEvent.Result;
//         }
//
//         return null;
//     }
//
//     public void Submit(ELidarTargetState currentState)
//     {
//         _quickTimeEvent?.Submit();
//     }
//
//     public void Cancel()
//     {
//         if (_quickTimeEvent == null)
//         {
//             return;
//         }
//
//         if (_quickTimeEvent.IsPlaying == true && _quickTimeEvent.IsFinished == false)
//         {
//             _quickTimeEvent.Cancel();
//         }
//     }
//
//     private void ResetNextTrigger()
//     {
//         _nextMinigameTriggerProgress = Random.Range(
//             _settings.MinMinigameInterval,
//             _settings.MaxMinigameInterval
//         );
//     }
}