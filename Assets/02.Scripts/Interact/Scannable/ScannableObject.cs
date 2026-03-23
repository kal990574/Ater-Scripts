using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScannableObject : MonoBehaviour, IQTEInvoker
{
    [Header("Required References")]
    [SerializeField] private ScanProgressSetting _settings;

    [Header("Debug")]
    [SerializeField] private float _currentQTEDelay;

    private ScanProgress _progress;
    private ScanStateMachine _fsm;

    public float CurrentQTEDelay => _currentQTEDelay;
    public ScanProgressSetting Settings => _settings;
    public bool IsProgressComplete => _progress != null && _progress.IsActivated;
    public bool CanInteract => _progress != null && _progress.CanInteract;
    public float CurrentProgress => _progress != null ? _progress.CurrentProgress : 0.0f;
    public float RequiredProgress => _progress != null ? _progress.RequiredProgress : 0.0f;
    public float ProgressRatio => _progress != null ? _progress.ProgressRatio : 0.0f;
    public EScannableState State => _fsm.CurrentStateType;

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
        if (IsProgressComplete)
        {
            return;
        }

        _fsm.Tick(Time.deltaTime);
    }

    public void Init()
    {
        if (_settings == null)
        {
            Debug.LogError($"[{nameof(ScannableObject)}] {nameof(ScanProgressSetting)} is missing.", this);
            enabled = false;
            return;
        }

        if (_progress != null)
        {
            _progress.OnProgressChanged -= HandleProgressChanged;
            _progress.OnActivated -= HandleActivated;
        }

        _progress = new ScanProgress(_settings);
        _progress.OnProgressChanged += HandleProgressChanged;
        _progress.OnActivated += HandleActivated;

        SetQTEDelay();
        _fsm = new ScanStateMachine(this);
        ChangeState(EScannableState.Default, true);
    }

    [ContextMenu("Reset")]
    public void ResetAll()
    {
        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.CancelByOwner(this);
        }

        _progress?.Reset();
        SetQTEDelay();
        ChangeState(EScannableState.Default, true);
    }

    public void OnScanning(float deltaTime)
    {
        if (IsProgressComplete)
        {
            return;
        }

        _fsm.OnScanning(deltaTime);
    }

    public void OnScanLost()
    {
        if (IsProgressComplete)
        {
            return;
        }

        _fsm.OnScanLost();
    }

    public void ChangeState(EScannableState nextState, bool force = false)
    {
        _fsm.ChangeState(nextState, force);
    }

    public void ChangeState(EScannableState nextState)
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
        TransitToDefaultIfEmpty();
    }

    public void ApplyScanProgress(float deltaTime)
    {
        AddProgress(deltaTime);
    }

    public void ApplyGreatSuccessBonus()
    {
        AddProgress(_settings.GreatSuccessBonus);
    }

    public void HandleQteFailure()
    {
        ReduceProgress(_settings.FailPenalty);
        ChangeState(CurrentProgress <= 0.0f ? EScannableState.Default : EScannableState.OnReturn);
    }

    public bool TryTransitToCompleted()
    {
        if (IsProgressComplete == false)
        {
            return false;
        }

        ChangeState(EScannableState.OnCompleted);
        return true;
    }

    public void TransitToProgressOrCompleted()
    {
        ChangeState(IsProgressComplete ? EScannableState.OnCompleted : EScannableState.OnProgress);
    }

    public void TransitToDefaultIfEmpty()
    {
        if (CurrentProgress <= 0.0f)
        {
            ChangeState(EScannableState.Default);
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
