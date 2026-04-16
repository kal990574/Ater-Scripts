using System;

public class ScannableScanController : IScanStateContext, IScannableQTEHandler, IDisposable
{
    private readonly ScanProgressSettingSO _setting;
    private readonly Action<float> _onProgressChanged;
    private readonly Action _onScanCompleted;

    private readonly ScanProgress _progress;
    private readonly ScanFSM _fsm;

    public bool IsProgressComplete => _progress.IsActivated;
    public float CurrentProgress => _progress.CurrentProgress;
    public float ProgressRatio => _progress.ProgressRatio;
    public EScanState State => _fsm.CurrentStateType;

    public ScannableScanController(
        ScanProgressSettingSO setting,
        Action<float> onProgressChanged,
        Action onScanCompleted)
    {
        _setting = setting;
        _onProgressChanged = onProgressChanged;
        _onScanCompleted = onScanCompleted;

        _progress = new ScanProgress(_setting);
        _progress.OnProgressChanged += HandleProgressChanged;
        _progress.OnActivated += HandleScanCompleted;

        _fsm = new ScanFSM(this);
        ChangeState(EScanState.Default, true);
    }

    public void Dispose()
    {
        _progress.OnProgressChanged -= HandleProgressChanged;
        _progress.OnActivated -= HandleScanCompleted;
    }

    public void Tick(float deltaTime)
    {
        if (IsProgressComplete)
        {
            return;
        }

        _fsm.Tick(deltaTime);
    }

    public void OnScanning(float deltaTime)
    {
        if (IsProgressComplete)
        {
            return;
        }

        _fsm.OnScanning(deltaTime);
    }

    public void OnScanStopped()
    {
        if (IsProgressComplete)
        {
            return;
        }

        _fsm.OnScanStopped();
    }

    public void Reset()
    {
        _progress.Reset();
        ChangeState(EScanState.Default, true);
    }

    public void ForceComplete()
    {
        _progress.Complete();
        ChangeState(EScanState.OnCompleted, true);
    }

    public void RestoreCompletedState()
    {
        _progress.Complete(false);
        ChangeState(EScanState.OnCompleted, true);
    }

    public void ChangeState(EScanState nextState, bool force = false)
    {
        _fsm.ChangeState(nextState, force);
    }

    public void ApplyScanProgress(float deltaTime)
    {
        _progress.Add(deltaTime);
    }

    public void ReduceProgressByReturn(float deltaTime)
    {
        _progress.Reduce(_setting.DecaySpeed * deltaTime);
        if (CurrentProgress <= 0.0f)
        {
            ChangeState(EScanState.Default);
        }
    }

    public bool TryTransitToCompleted()
    {
        if (!IsProgressComplete)
        {
            return false;
        }

        ChangeState(EScanState.OnCompleted);
        return true;
    }

    public void PauseScanning()
    {
        if (IsProgressComplete)
        {
            return;
        }

        ChangeState(EScanState.OnHold);
    }

    public void ResumeScanningAfterQte()
    {
        if (IsProgressComplete)
        {
            return;
        }

        ChangeState(EScanState.OnProgress);
    }

    public bool HandleQteSuccess()
    {
        if (IsProgressComplete)
        {
            ChangeState(EScanState.OnCompleted);
            return true;
        }

        ChangeState(EScanState.OnProgress);
        return false;
    }

    public bool HandleQteGreatSuccess(float greatSuccessBonus)
    {
        if (IsProgressComplete)
        {
            ChangeState(EScanState.OnCompleted);
            return true;
        }

        _progress.Add(greatSuccessBonus);
        if (TryTransitToCompleted())
        {
            return true;
        }

        ChangeState(EScanState.OnProgress);
        return false;
    }

    public void HandleQteFailure(float failPenalty)
    {
        if (IsProgressComplete)
        {
            return;
        }

        _progress.Reduce(failPenalty);
        ChangeState(CurrentProgress <= 0.0f ? EScanState.Default : EScanState.OnReturn);
    }

    private void HandleProgressChanged(float ratio)
    {
        _onProgressChanged?.Invoke(ratio);
    }

    private void HandleScanCompleted()
    {
        _onScanCompleted?.Invoke();
    }
}
