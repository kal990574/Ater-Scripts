using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class InteractableObject : DetectableObject, IInteractObject, IItemBindable
{
    [SerializeField] protected bool _isInteractActive;
    private IItemInstance _instance;

    public int InitialItemKey { get; }
    public InstanceData InstanceData => _instance.InstanceData;
    public override bool CanDetect => _isDetectable && _isInteractActive;

    public event Action OnInteract;

    [Header("Scene Event")]
    public UnityEvent InteractEvent;

    private void Awake()
    {
        if (TryGetComponent(out IItemInstance instance))
        {
            _instance = instance;
        }
    }
    

    public abstract void Interact(UseContext context);

    private void SetActivate()
    {
        SetActivate(true);
    }

    public void SetActivate(bool active)
    {
        _isInteractActive = active;
        if (!_isInteractActive && _isOnDetected)
        {
            OnDetectExit();
        }
    }

    public void SetInstance(IItemInstance itemInstance)
    {
        _instance = itemInstance;
    }

    protected void OnInteractActivate()
    {
        OnInteract?.Invoke();
        InteractEvent?.Invoke();
    }
}
