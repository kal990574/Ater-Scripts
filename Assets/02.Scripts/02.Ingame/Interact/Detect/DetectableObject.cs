using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class DetectableObject : MonoBehaviour , IDetectable
{
    [SerializeField] protected bool _isDetectable = true;
    protected bool _isOnDetected = false;

    public Transform Transform => transform;
    public virtual bool CanDetect => _isDetectable;

    [SerializeField] private string _hoverDescription;
    public virtual string HoverDescription =>  _hoverDescription;

    public virtual bool CanShowHoverUI => _isDetectable;

    public event Action<bool> OnDetected;

    [Header("Detected Event")]
    public UnityEvent DetectOnEvent;
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
