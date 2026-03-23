using System;
using UnityEngine;

public class ScannableObject : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private ScanProgressSetting _settings;

    private ScanProgress _progress;
    private ScanStateMachine _fsm;

    public ScanProgressSetting Settings => _settings;
    public bool IsProgressComplete => _progress != null && _progress.IsActivated;
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

        _fsm = new ScanStateMachine(this);
        ChangeState(EScannableState.Default, true);
    }

    [ContextMenu("Reset")]
    public void ResetAll()
    {
        _progress?.Reset();
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
        if (CurrentProgress <= 0.0f)
        {
            ChangeState(EScannableState.Default);
        }
    }

    public void ApplyScanProgress(float deltaTime)
    {
        AddProgress(deltaTime);
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

    public void PauseScanning()
    {
        if (IsProgressComplete)
        {
            return;
        }

        ChangeState(EScannableState.OnHold);
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
