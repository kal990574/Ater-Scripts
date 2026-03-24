using System;
using UnityEngine;
using UnityEngine.Events;

public class OutlineTarget : MonoBehaviour, IInteractTarget
{
    public Transform Target => transform;
    public event Action<bool> OnTargetDetected;
    
    [ContextMenu("hover")]
    public void OnTargetDetectEnter()
    {
        OnTargetDetected?.Invoke(true);
    }

    [ContextMenu("unhover")]
    public void OnTargetDetectExit()
    {
        OnTargetDetected?.Invoke(false);
    }
}
