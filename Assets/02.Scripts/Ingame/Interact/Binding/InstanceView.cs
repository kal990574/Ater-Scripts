using System;
using UnityEngine;

public class InstanceView : MonoBehaviour, IItemInstance
{
    [SerializeField] private string _instanceId;
    [SerializeField] private int _initialItemKey = -1;
    [SerializeField] private bool _InstanceOnInit = false;
    
    private InventoryManager _inventoryManager;

    public string InstanceId => _instanceId;
    public InstanceData InstanceData => ResolveItemInstance();
    public InventoryManager InventoryManager => _inventoryManager;
    public int InitialItemKey => _initialItemKey;

    private void Start()
    {
        if (_InstanceOnInit && InventoryManager.Instance != null)
        {
            InstanceData instanceData = InventoryManager.Instance.CreateItemInstance(_initialItemKey);
            Bind(instanceData != null ? instanceData.InstanceId : null, InventoryManager.Instance);
            ActivateInstanceIfNeeded();
        }
    }

    public virtual void Bind(string instanceId, InventoryManager inventoryManager)
    {
        _instanceId = instanceId;
        _inventoryManager = inventoryManager;
        PropagateItemInstance();
        RefreshView();
    }

    public InstanceData EnsureItemInstance()
    {
        InstanceData instanceData = ResolveItemInstance();
        if (instanceData != null)
        {
            return instanceData;
        }

        InventoryManager inventoryManager = ResolveInventoryManager();
        if (inventoryManager == null)
        {
            Debug.LogError($"[{nameof(InstanceView)}] {nameof(InventoryManager)}.Instance is null.", this);
            return null;
        }

        if (_initialItemKey < 0)
        {
            Debug.LogError($"[{nameof(InstanceView)}] Initial item key is missing or invalid.", this);
            return null;
        }

        instanceData = inventoryManager.CreateItemInstance(_initialItemKey);
        if (instanceData == null)
        {
            return null;
        }

        _instanceId = instanceData.InstanceId;
        _inventoryManager = inventoryManager;
        PropagateItemInstance();
        ActivateInstanceIfNeeded();
        return instanceData;
    }

    public void RefreshView()
    {
        EnsureItemInstance();

        IBindApplier binder = GetComponentInChildren<IBindApplier>();
        if (binder == null)
        {
            return;
        }

        binder.ApplyState(this);
    }

    private InventoryManager ResolveInventoryManager()
    {
        if (_inventoryManager != null)
        {
            return _inventoryManager;
        }

        _inventoryManager = InventoryManager.Instance;
        return _inventoryManager;
    }
    
    private void PropagateItemInstance()
    {
        IItemBindable[] itemBindables = GetComponentsInChildren<IItemBindable>(true);
        foreach (IItemBindable itemBindable in itemBindables)
        {
            itemBindable?.SetInstance(this);
        }
    }

    private InstanceData ResolveItemInstance()
    {
        InventoryManager inventoryManager = ResolveInventoryManager();
        if (inventoryManager == null || string.IsNullOrEmpty(_instanceId))
        {
            return null;
        }

        return inventoryManager.GetItemInstance(_instanceId);
    }

    private void ActivateInstanceIfNeeded()
    {
        if (_InstanceOnInit && !string.IsNullOrEmpty(_instanceId))
        {
            gameObject.SetActive(true);
        }
    }
}
