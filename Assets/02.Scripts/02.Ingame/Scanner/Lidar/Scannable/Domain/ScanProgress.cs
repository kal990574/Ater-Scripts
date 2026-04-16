using System;
using UnityEngine;
public class ScanProgress
{
    private readonly ScanProgressSettingSO _settingsSo;

    private float _currentProgress;

    public bool IsActivated { get; private set; }
    public bool CanInteract { get; private set; }

    public float CurrentProgress => _currentProgress;
    public float ProgressRatio => Mathf.Clamp01(_currentProgress / _settingsSo.MaxScanAmount);
    
    public event Action<float> OnProgressChanged;
    public event Action OnActivated;

    public ScanProgress(ScanProgressSettingSO settingsSo)
    {
        _settingsSo = settingsSo;
        Reset();
    }

    public void Add(float amount)
    {
        if (amount <= 0.0f || IsActivated)
        {
            return;
        }

        _currentProgress = Mathf.Clamp(_currentProgress + amount, 0.0f, _settingsSo.MaxScanAmount);

        NotifyProgressChanged();

        if (_currentProgress >= _settingsSo.MaxScanAmount)
        {
            Activate();
        }
    }

    public void Reduce(float amount)
    {
        if (amount <= 0.0f || IsActivated)
        {
            return;
        }

        _currentProgress = Mathf.Clamp(_currentProgress - amount, 0.0f, _settingsSo.MaxScanAmount);
        NotifyProgressChanged();
    }

    public void Reset()
    {
        IsActivated = false;
        CanInteract = false;
        _currentProgress = 0.0f;
        NotifyProgressChanged();
    }

    public void Complete(bool notify = true)
    {
        if (IsActivated)
        {
            return;
        }

        Activate(notify);
    }

    private void Activate(bool notify = true)
    {
        IsActivated = true;
        CanInteract = true;
        _currentProgress = _settingsSo.MaxScanAmount;

        NotifyProgressChanged();
        if (notify)
        {
            OnActivated?.Invoke();
        }
    }

    private void NotifyProgressChanged()
    {
        OnProgressChanged?.Invoke(ProgressRatio);
    }
}
