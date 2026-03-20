using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LidarTarget : MonoBehaviour, IQTEInvoker
{
    [Header("Settings")]
    [SerializeField] private LidarProgressSetting _settings;
    
    private LidarProgress _progress;
    private LidarStateMachine _fsm;
    
    private float _currentQTEDelay;
    public float CurrentQTEDelay => _currentQTEDelay;
    
    public LidarProgressSetting Settings => _settings;

    public bool IsProgressComplete => _progress != null && _progress.IsActivated;
    public bool CanInteract => _progress != null && _progress.CanInteract;
    public float CurrentProgress => _progress != null ? _progress.CurrentProgress : 0.0f;
    public float RequiredProgress => _progress != null ? _progress.RequiredProgress : 0.0f;
    public float ProgressRatio => _progress != null ? _progress.ProgressRatio : 0.0f;

    public ELidarTargetState State => _fsm.CurrentStateType;

    public event Action<float> OnProgressChanged;
    public event Action OnScanComplete;

    private void Awake()
    {
        Init();
        
    }

    private void OnDestroy()
    {
        if (_progress != null)
        {
            _progress.OnProgressChanged -= HandleProgressChanged;
            _progress.OnActivated -= HandleActivated;
        }

        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.CancelByOwner(this);
        }
    }

    private void Update()
    {
        if (IsProgressComplete == true)
        {
            return;
        }

        _fsm.Tick(Time.deltaTime);
    }

    public void Init()
    {
        _progress = new LidarProgress(_settings);
        _progress.OnProgressChanged += HandleProgressChanged;
        _progress.OnActivated += HandleActivated;

        SetQTEDelay();
        _fsm = new LidarStateMachine(this);
        ChangeState(ELidarTargetState.Default, true);
    }

    
    
    [ContextMenu("리셋")]
    public void ResetAll()
    {
        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.CancelByOwner(this);
        }

        _progress.Reset();
        ChangeState(ELidarTargetState.Default);
    }

    public void OnScanning(float deltaTime)
    {
        if (IsProgressComplete == true)
        {
            return;
        }

        _fsm.OnScanning(deltaTime);
    }

    public void OnScanLost()
    {
        if (IsProgressComplete == true)
        {
            return;
        }

        _fsm.OnScanLost();
    }

    public void ChangeState(ELidarTargetState nextState, bool force = false)
    {
        _fsm.ChangeState(nextState, force);
    }

    public void ChangeState(ELidarTargetState nextState)
    {
        ChangeState(nextState, false);
    }

    public void AddProgress(float amount)
    {
        _progress.Add(amount);
    }

    public void ReduceProgress(float amount)
    {
        _progress.Reduce(amount);
    }

    public void ReduceProgressByReturn(float deltaTime)
    {
        _progress.Reduce(_settings.ReturnSpeed * deltaTime);

        if (_progress.CurrentProgress <= 0.0f)
        {
            ChangeState(ELidarTargetState.Default);
        }
    }
    
    public void SetQTEDelay()
    {
        _currentQTEDelay = Random.Range(_settings.MinMinigameInterval, _settings.MaxMinigameInterval);
    }

    private void HandleProgressChanged(float ratio)
    {
        OnProgressChanged?.Invoke(ratio);
    }

    private void HandleActivated()
    {
        OnScanComplete?.Invoke();
    }
}