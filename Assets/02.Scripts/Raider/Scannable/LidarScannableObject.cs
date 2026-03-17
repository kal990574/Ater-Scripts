using System;
using System.Collections.Generic;
using UnityEngine;

public class LidarScannableObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LidarProgressSetting _settings;

    [Header("Minigame")]
    [SerializeField] private MonoBehaviour _minigameProvider;
    private IScanMinigame _minigame;
    
    [Header("FSM")]
    private readonly Dictionary<ELidarObjectState, ILidarScannableState> _states = new Dictionary<ELidarObjectState, ILidarScannableState>();
    private ILidarScannableState _currentState;

    [Header("Cache")]
    private float _currentProgress = 0.0f;
    private float _progressSinceLastMinigame = 0.0f;
    private float _nextMinigameTriggerProgress = 0.0f;
    
    
    //프로퍼티
    public bool IsActivated { get; private set; }
    public bool CanInteract { get; private set; }
    public ELidarObjectState State => _currentState == null ?  ELidarObjectState.Default : _currentState.StateType;
    public float CurrentProgress => _currentProgress;
    public float RequiredProgress => _settings.RequiredScanTime;
    public float ProgressRatio => Mathf.Clamp01(_currentProgress / _settings.RequiredScanTime);
    public LidarProgressSetting Settings => _settings;
    public IScanMinigame Minigame => _minigame;
    

    public event Action<ELidarObjectState> OnStateChanged;
    public event Action<float> OnProgressChanged;
    public event Action OnActivated;

    private void Awake()
    {
        _minigame = _minigameProvider as IScanMinigame;

        if (_minigameProvider != null && _minigame == null)
        {
            throw new Exception($"[{nameof(LidarScannableObject)}] Assigned minigame provider must implement {nameof(IScanMinigame)}.");
        }
        ResetNextMinigameTrigger();
        ChangeState(ELidarObjectState.Default, true);
    }

    private void Update()
    {
        if (IsActivated == true)
        {
            return;
        }

        _currentState?.Tick(Time.deltaTime);
    }
    
    private void ResetNextMinigameTrigger()
    {
        _progressSinceLastMinigame = 0.0f;
        _nextMinigameTriggerProgress = UnityEngine.Random.Range(
            _settings.MinMinigameInterval,
            _settings.MaxMinigameInterval
        );
    }

    public void OnScanning(float deltaTime)
    {
        if (IsActivated == true)
        {
            return;
        }

        _currentState?.OnScanning(deltaTime);
    }

    public void OnScanLost()
    {
        if (IsActivated == true)
        {
            return;
        }

        _currentState?.OnScanLost();
    }

    public void SubmitMinigame()
    {
        if (State != ELidarObjectState.OnMinigame)
        {
            return;
        }

        _minigame?.Submit();
    }

    public void ResetScannable()
    {
        if (State == ELidarObjectState.OnMinigame)
        {
            _minigame?.Cancel();
        }

        IsActivated = false;
        CanInteract = false;
        _currentProgress = 0.0f;
        _progressSinceLastMinigame = 0.0f;

        NotifyProgressChanged();
        ChangeState(ELidarObjectState.Default);
    }

    public void ChangeState(ELidarObjectState nextState, bool force = false)
    {
        if (force == false && _currentState != null && _currentState.StateType == nextState)
        {
            return;
        }

        if (_states.ContainsKey(nextState) == false)
        {
            InstanceNewState(nextState);
        }
        
        _currentState?.Exit();

        _currentState = _states[nextState];
        _currentState.Enter();

        OnStateChanged?.Invoke(nextState);
    }

    private void InstanceNewState(ELidarObjectState nextState)
    {
        switch(nextState)
        {
            case ELidarObjectState.Default:  
                _states[ELidarObjectState.Default] = new LidarDefaultState(this); 
                break;
            case ELidarObjectState.OnProgress:  
                _states[ELidarObjectState.OnProgress] = new LidarProgressState(this); 
                break;
            case ELidarObjectState.OnReturn:  
                _states[ELidarObjectState.OnReturn] = new LidarReturnState(this); 
                break;
            case ELidarObjectState.OnMinigame:  
                _states[ELidarObjectState.OnMinigame] = new LidarMinigameState(this); 
                break;
        }
    }

    public void AddProgress(float amount)
    {
        if (amount <= 0.0f)
        {
            return;
        }

        _currentProgress = Mathf.Clamp(_currentProgress + amount, 0.0f, _settings.RequiredScanTime);
        _progressSinceLastMinigame += amount;

        NotifyProgressChanged();

        if (_currentProgress >= _settings.RequiredScanTime)
        {
            ActivateObject();
            return;
        }

        if (ShouldEnterMinigame() == true)
        {
            ChangeState(ELidarObjectState.OnMinigame);
        }
    }

    public void ReduceProgress(float amount)
    {
        if (amount <= 0.0f)
        {
            return;
        }

        _currentProgress = Mathf.Clamp(_currentProgress - amount, 0.0f, _settings.RequiredScanTime);
        NotifyProgressChanged();
    }

    public void ReduceProgressByReturn(float deltaTime)
    {
        ReduceProgress(_settings.ReturnSpeed * deltaTime);

        if (_currentProgress <= 0.0f)
        {
            ChangeState(ELidarObjectState.Default);
        }
    }

    public bool ShouldEnterMinigame()
    {
        if (_minigame == null)
        {
            return false;
        }

        if (State != ELidarObjectState.OnProgress)
        {
            return false;
        }

        if (_progressSinceLastMinigame < _nextMinigameTriggerProgress)
        {
            return false;
        }

        ResetNextMinigameTrigger();
        return true;
    }

    public void BeginMinigame()
    {
        if (_minigame == null)
        {
            ChangeState(ELidarObjectState.OnProgress);
            return;
        }

        _minigame.Begin();
    }

    public void TickMinigame(float deltaTime)
    {
        if (_minigame == null)
        {
            ChangeState(ELidarObjectState.OnProgress);
            return;
        }

        _minigame.Tick(deltaTime);

        if (_minigame.IsFinished == true)
        {
            ApplyMinigameResult(_minigame.Result);
        }
    }

    public void EndMinigame()
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

    public void ApplyMinigameResult(MinigameResult result)
    {
        switch (result)
        {
            case MinigameResult.None:
            {
                ChangeState(ELidarObjectState.OnProgress);
                break;
            }
            case MinigameResult.Fail:
            {
                ReduceProgress(_settings.FailPenalty);

                if (_currentProgress > 0.0f)
                {
                    ChangeState(ELidarObjectState.OnReturn);
                }
                else
                {
                    ChangeState(ELidarObjectState.Default);
                }

                break;
            }
            case MinigameResult.Success:
            {
                ChangeState(ELidarObjectState.OnProgress);
                break;
            }
            case MinigameResult.GreatSuccess:
            {
                AddProgress(_settings.GreatSuccessBonus);

                if (IsActivated == false)
                {
                    ChangeState(ELidarObjectState.OnProgress);
                }

                break;
            }
        }
    }
    
    public void FailCurrentMinigame()
    {
        if (State != ELidarObjectState.OnMinigame)
        {
            return;
        }

        if (_minigame != null && _minigame.IsFinished == false)
        {
            _minigame.ForceFail();
        }

        ApplyMinigameResult(MinigameResult.Fail);
    }

    private void ActivateObject()
    {
        IsActivated = true;
        CanInteract = true;
        _currentProgress = _settings.RequiredScanTime;

        NotifyProgressChanged();
        OnActivated?.Invoke();
    }

    private void NotifyProgressChanged()
    {
        OnProgressChanged?.Invoke(ProgressRatio);
    }
}