using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class Interactable : DetectableObject, IRuntimeInteractObject, IRuntimeDataConsumer
{
    [SerializeField] protected bool _isInteractActive;
    private IRuntimeView _instance;
    private ScannableObject _scannableObject;

    public RuntimeData RuntimeData => _instance != null ? _instance.RuntimeData : null;
    public RuntimeItemData RuntimeItemData => _instance.RuntimeItemData;
    public bool IsInteractActive => _isInteractActive && IsScanRequirementSatisfied() && IsAdditionalInteractRequirementSatisfied();
    public override bool CanDetect => _isDetectable && IsInteractActive;
    protected IRuntimeView RuntimeView => _instance;

    public event Action OnInteract;

    [Header("Scene Event")]
    public UnityEvent InteractEvent;

    private void Awake()
    {
        if (TryGetComponent(out IRuntimeView instance))
        {
            _instance = instance;
        }

        CacheScannableObject();
        SubscribeScannableEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeScannableEvents();
    }


    public abstract void Interact(InteractionContext context);

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
        RefreshInteractAvailability();
    }

    public void SetRuntimeData(IRuntimeView runtimeView)
    {
        _instance = runtimeView;
    }

    protected void OnInteractActivate()
    {
        OnInteract?.Invoke();
        InteractEvent?.Invoke();
    }

    protected void RefreshRuntimeView()
    {
        _instance?.RefreshView();
    }

    protected virtual bool IsAdditionalInteractRequirementSatisfied()
    {
        return true;
    }

    protected void RefreshInteractAvailability()
    {
        if (!IsInteractActive && _isOnDetected)
        {
            OnDetectExit();
        }
    }

    private bool IsScanRequirementSatisfied()
    {
        if (_scannableObject == null)
        {
            return true;
        }

        return _scannableObject.IsProgressComplete;
    }

    private void CacheScannableObject()
    {
        if (_scannableObject != null)
        {
            return;
        }

        _scannableObject = GetComponent<ScannableObject>();
        if (_scannableObject == null)
        {
            _scannableObject = GetComponentInParent<ScannableObject>();
        }

        if (_scannableObject == null)
        {
            _scannableObject = GetComponentInChildren<ScannableObject>();
        }
    }

    private void SubscribeScannableEvents()
    {
        if (_scannableObject == null)
        {
            return;
        }

        _scannableObject.OnScanComplete -= HandleScanComplete;
        _scannableObject.OnScanComplete += HandleScanComplete;
    }

    private void UnsubscribeScannableEvents()
    {
        if (_scannableObject == null)
        {
            return;
        }

        _scannableObject.OnScanComplete -= HandleScanComplete;
    }

    private void HandleScanComplete()
    {
        RefreshInteractAvailability();
    }
}
