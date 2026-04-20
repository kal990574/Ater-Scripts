using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class Interactable : DetectableObject, IRuntimeInteractObject, IRuntimeDataConsumer
{
    public enum EAfterInteract
    {
        None,
        Deactive,
        Disable,
        Destroy
    }
    
    [TabGroup("Inspector", "Interactable")]
    [SerializeField] protected bool _isInteractActive;

    [TabGroup("Inspector", "Interactable")] 
    [LabelText("Interact Collider Layer")] 
    [SerializeField]
    private LayerMask _interactColliderLayers;

    private IRuntimeView _instance;
    protected ScannableObject _scannableObject;
    private GameEventPublisher _eventPublisher;
    private List<Collider> _interactColliders = new List<Collider>();

    [TabGroup("Inspector", "Interactable")]
    [LabelText("Hover Description Before Scan")]
    [MultiLineProperty]
    [SerializeField] private string _hoverDescriptionBeforeScan = "";

    public RuntimeData RuntimeData => _instance != null ? _instance.RuntimeData : null;
    public RuntimeItemData RuntimeItemData => _instance.RuntimeItemData;
    public bool IsInteractActive => _isInteractActive && IsScanRequirementSatisfied() && IsAdditionalInteractRequirementSatisfied();
    public override bool CanDetect => _isDetectable && IsInteractActive;
    public override bool CanShowHoverUI => _isDetectable && _isInteractActive;

    protected IRuntimeView RuntimeView => _instance;

    public event Action OnInteract;

    [TabGroup("Inspector", "Events")]
    [LabelText("On Interaction Success")]
    [SerializeField] protected UnityEvent _onInteractionSuccess;

    [TabGroup("Inspector", "Events")]
    [LabelText("On Interaction Failed")]
    [SerializeField] protected UnityEvent _onInteractionFailed;

    [TabGroup("Inspector", "Debug")]
    [ShowInInspector, ReadOnly, LabelText("Interact Collider Count")]
    private int DebugInteractColliderCount => _interactColliders?.Count ?? 0;

    private void Awake()
    {
        _eventPublisher = new GameEventPublisher();
        _eventPublisher.SetSource(this);

        if (TryGetComponent(out IRuntimeView instance))
        {
            _instance = instance;
        }

        CacheInteractColliders();
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
        SetInteractCollidersEnabled(active);
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

    private void CacheInteractColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        _interactColliders.Clear();

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            if (collider == null)
            {
                continue;
            }

            int colliderLayerMask = 1 << collider.gameObject.layer;
            if ((_interactColliderLayers.value & colliderLayerMask) == 0)
            {
                continue;
            }

            _interactColliders.Add(collider);
        }

        SetInteractCollidersEnabled(true);
    }
    
    private void SetInteractCollidersEnabled(bool isEnabled)
    {
        if (_interactColliders == null || _interactColliders.Count == 0)
        {
            return;
        }

        for (int i = 0; i < _interactColliders.Count; i++)
        {
            Collider collider = _interactColliders[i];
            if (collider == null)
            {
                continue;
            }

            collider.enabled = isEnabled;
        }
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
