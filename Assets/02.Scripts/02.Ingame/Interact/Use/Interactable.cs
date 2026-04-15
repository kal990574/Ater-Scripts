using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class Interactable : DetectableObject, IRuntimeInteractObject, IRuntimeDataConsumer
{
    [SerializeField] protected bool _isInteractActive;
    private IRuntimeView _instance;
    protected ScannableObject _scannableObject;
    private GameEventPublisher _eventPublisher;

    [SerializeField] private string _hoverDescriptionBeforeScan = "";

    public RuntimeData RuntimeData => _instance != null ? _instance.RuntimeData : null;
    public RuntimeItemData RuntimeItemData => _instance.RuntimeItemData;
    public bool IsInteractActive => _isInteractActive && IsScanRequirementSatisfied() && IsAdditionalInteractRequirementSatisfied();
    public override bool CanDetect => _isDetectable && IsInteractActive;
    public override bool CanShowHoverUI => _isDetectable && _isInteractActive;
    public override string HoverDescription
    {
        get
        {
            if (!IsScanRequirementSatisfied()) return _hoverDescriptionBeforeScan; 
            if (!_isInteractActive) return string.Empty;                           
            return base.HoverDescription;                                          
        }
    }
    protected IRuntimeView RuntimeView => _instance;

    public event Action OnInteract;

    [Header("Scene Event")]
    [SerializeField] protected UnityEvent _onInteractionSuccess;
    [SerializeField] protected UnityEvent _onInteractionFailed;
    private void Awake()
    {
        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);

        if (TryGetComponent(out IRuntimeView instance))
        {
            _instance = instance;
        }

        CacheScannableObject();
        SubscribeScannableEvents();
        OnAwake();
    }

    private void OnDestroy()
    {
        OnBeforeDestroy();
        UnsubscribeScannableEvents();
    }


    [Button]
    public abstract void Interact(InteractionContext context);

    protected virtual void OnAwake()
    {
    }

    protected virtual void OnBeforeDestroy()
    {
    }

    private void SetActivate()
    {
        SetActivate(true);
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
        _onInteractionSuccess?.Invoke();
    }

    protected void PublishObjectInteracted(EInteractObjectEventType type)
    {
        _eventPublisher?.TryPublish(context => new ObjectInteractedRawEvent(context, type));
    }

    protected void PublishItemAcquired(int itemId)
    {
        _eventPublisher?.TryPublish(context => new ItemAcquiredRawEvent(context, itemId,RuntimeItemData.ItemType ));
    }

    protected void RefreshRuntimeView()
    {
        _instance?.RefreshView();
    }

    protected T GetComponentCached<T>(ref T field) where T : Component
    {
        if (field == null)
        {
            field = GetComponent<T>();
        }

        return field;
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
