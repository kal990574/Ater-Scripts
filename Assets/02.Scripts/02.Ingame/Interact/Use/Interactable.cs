using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class Interactable : DetectableObject, IInteractObject, INeedRuntimeData
{
    [SerializeField] protected bool _isInteractActive;
    private IRuntimeView _instance;
    
    public RuntimeData RuntimeData => _instance != null ? _instance.RuntimeData : null;
    public RuntimeItemData RuntimeItemData => _instance.RuntimeItemData;
    public override bool CanDetect => _isDetectable && _isInteractActive;
    
    private GameEventPublisher _publisher;

    public event Action OnInteract;

    [Header("Scene Event")]
    public UnityEvent InteractEvent;

    private void Awake()
    {
        if (TryGetComponent(out IRuntimeView instance))
        {
            _instance = instance;
        }

        _publisher = new GameEventPublisher();
        _publisher.SetSource(this);
    }
    

    public abstract void Interact(UseContext context);

    private void SetActivate()
    {
        SetActivate(true);
    }

    public void SetDetectable(bool isDetectable)
    {
        _isDetectable = isDetectable;
    }

    public void SetActivate(bool active)
    {
        _isInteractActive = active;
        if (!_isInteractActive && _isOnDetected)
        {
            OnDetectExit();
        }
    }

    public void SetRuntimeData(IRuntimeView runtimeView)
    {
        _instance = runtimeView;
    }

    protected void OnInteractActivate()
    {
        OnInteract?.Invoke();
        InteractEvent?.Invoke();
        _publisher.TryPublish(ctx => new InteractedRawEvent(ctx));
    }
}
