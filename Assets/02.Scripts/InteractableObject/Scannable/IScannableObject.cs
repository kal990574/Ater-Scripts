using System;
using UnityEngine;

//스캔이 가능한 대상
public interface IScannableObject
{
    void OnScanStarted();
    void OnScanning(float deltaTime);
    void OnScanStopped();
    event Action<float> OnScanProgressChanged;
    event Action OnScanComplete;
}