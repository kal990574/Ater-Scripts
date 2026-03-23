using System;
using UnityEngine;
public class LidarProgress
{
    private readonly LidarProgressSetting _settings;

    private float _currentProgress;
    private float _progressSinceLastMinigame;

    public bool IsActivated { get; private set; }
    public bool CanInteract { get; private set; }

    public float CurrentProgress => _currentProgress;
    public float RequiredProgress => _settings.RequiredScanTime;
    public float ProgressRatio => Mathf.Clamp01(_currentProgress / _settings.RequiredScanTime);
    public float ProgressSinceLastMinigame => _progressSinceLastMinigame;

    
    public event Action<float> OnProgressChanged;
    public event Action OnActivated;

    public LidarProgress(LidarProgressSetting settings)
    {
        _settings = settings;
        Reset();
    }

    public void Add(float amount)
    {
        if (amount <= 0.0f || IsActivated)
        {
            return;
        }

        _currentProgress = Mathf.Clamp(_currentProgress + amount, 0.0f, _settings.RequiredScanTime);
        _progressSinceLastMinigame += amount;

        NotifyProgressChanged();

        if (_currentProgress >= _settings.RequiredScanTime)
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

        _currentProgress = Mathf.Clamp(_currentProgress - amount, 0.0f, _settings.RequiredScanTime);
        NotifyProgressChanged();
    }

    public void ConsumeMinigameProgress()
    {
        _progressSinceLastMinigame = 0.0f;
    }

    public void Reset()
    {
        IsActivated = false;
        CanInteract = false;
        _currentProgress = 0.0f;
        _progressSinceLastMinigame = 0.0f;
        NotifyProgressChanged();
    }

    private void Activate()
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