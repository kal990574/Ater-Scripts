using System;
using UnityEngine;

public interface IDetectable
{
    Transform Transform { get; }
    bool CanDetect { get; }
    bool CanShowHoverUI { get; }
    void OnDetectEnter();
    void OnDetectExit();
    string HoverDescription { get; }
    event Action<bool> OnDetected;
}
