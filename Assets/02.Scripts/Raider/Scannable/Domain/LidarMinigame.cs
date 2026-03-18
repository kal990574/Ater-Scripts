using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LidarMinigame
{
    private readonly LidarProgressSetting _settings;
    private readonly IScanMinigame _minigame;
    private readonly LidarProgress _progress;
    private readonly Action<ELidarTargetState> _changeState;

    private float _nextMinigameTriggerProgress;

    public LidarMinigame(
        LidarProgressSetting settings,
        IScanMinigame minigame,
        LidarProgress progress,
        Action<ELidarTargetState> changeState)
    {
        _settings = settings;
        _minigame = minigame;
        _progress = progress;
        _changeState = changeState;

        ResetNextTrigger();
    }
    
    
    //미니게임 진입 가능 검사 => 가능하면 미니게임 실행
    public bool ShouldEnterMinigame(ELidarTargetState currentState)
    {
        if (_minigame == null)
        {
            return false;
        }

        if (currentState != ELidarTargetState.OnProgress)
        {
            return false;
        }

        if (_progress.ProgressSinceLastMinigame < _nextMinigameTriggerProgress)
        {
            return false;
        }

        _progress.ConsumeMinigameProgress();
        ResetNextTrigger();
        return true;
    }

    public void BeginOrReturnToProgress()
    {
        if (_minigame == null)
        {
            _changeState(ELidarTargetState.OnProgress);
            return;
        }

        _minigame.Begin();
    }

    public MinigameResult? Tick(float deltaTime)
    {
        if (_minigame == null)
        {
            _changeState(ELidarTargetState.OnProgress);
            return null;
        }

        _minigame.Tick(deltaTime);

        if (_minigame.IsFinished == true)
        {
            return _minigame.Result;
        }

        return null;
    }

    public void Submit(ELidarTargetState currentState)
    {
        if (currentState != ELidarTargetState.OnMinigame)
        {
            return;
        }

        _minigame?.Submit();
    }

    public void CancelIfPlaying()
    {
        if (_minigame == null)
        {
            return;
        }

        if (_minigame.IsPlaying == true && _minigame.IsFinished == false)
        {
            _minigame.Cancel();
        }
    }

    public void ForceFail()
    {
        if (_minigame != null && _minigame.IsFinished == false)
        {
            _minigame.ForceFail();
        }
    }

    private void ResetNextTrigger()
    {
        _nextMinigameTriggerProgress = Random.Range(
            _settings.MinMinigameInterval,
            _settings.MaxMinigameInterval
        );
    }
}