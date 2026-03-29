using System;
using UnityEngine;

public interface IDetectableObject
{
    Transform Transform { get; }
    bool CanDetect { get; }
    void OnDetectEnter();
    void OnDetectExit();

    event Action<bool> OnDetected;
}
