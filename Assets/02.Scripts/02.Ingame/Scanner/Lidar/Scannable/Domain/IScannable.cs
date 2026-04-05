using System;

// Scannable target.
public interface IScannable
{
    bool IsProgressComplete { get; }

    float CurrentProgress { get; }

    float ProgressRatio { get; }

    event Action<float> OnScanProgressChanged;
    event Action OnScanComplete;
}
