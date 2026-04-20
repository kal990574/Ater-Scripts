using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class DetectableObject : MonoBehaviour , IDetectable
{
    [TabGroup("Inspector", "DetectableObject")]
    [ToggleLeft]
    [SerializeField] protected bool _isDetectable = true;
    protected bool _isOnDetected = false;

    public Transform Transform => transform;
    public virtual bool CanDetect => _isDetectable;

    public virtual bool CanShowHoverUI => _isDetectable;

    public event Action<bool> OnDetected;

    [TabGroup("Inspector", "DetectableObject")]
    [LabelText("On Detect Enter")]
    public UnityEvent DetectOnEvent;

    [TabGroup("Inspector", "DetectableObject")]
    [LabelText("On Detect Exit")]
    public UnityEvent DetectOffEvent;

    [ContextMenu("hover")]
    public void OnDetectEnter()
    {
        if (!CanDetect || _isOnDetected)
        {
            return;
        }

        Debug.Log("Hover");
        _isOnDetected = true;
        OnDetected?.Invoke(true);
        DetectOnEvent?.Invoke();
    }

    [ContextMenu("unhover")]
    public void OnDetectExit()
    {
        if (!_isOnDetected)
        {
            return;
        }

        OnDetected?.Invoke(false);
        DetectOffEvent?.Invoke();
        _isOnDetected = false;
    }

    
}
