using System;
using UnityEngine;

public class InstanceView : MonoBehaviour, IItemInstance
{
    [SerializeField] private string _instanceId;
    [SerializeField] private int _initialItemKey = -1;
    [SerializeField] private bool _InstanceOnInit = false;
    
    private InventoryManager _inventoryManager;

    public string InstanceId => _instanceId;
    public ItemInstanceData ItemInstanceData => ResolveItemInstance();
    public InventoryManager InventoryManager => _inventoryManager;
    public int InitialItemKey => _initialItemKey;

    private void Start()
    {
        if (_InstanceOnInit && InventoryManager.Instance != null && ResolveItemInstance() == null)
        {
            ItemInstanceData itemInstanceData = InventoryManager.Instance.CreateItemInstance(_initialItemKey);
            Bind(itemInstanceData != null ? itemInstanceData.InstanceId : null, InventoryManager.Instance);
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

    public ItemInstanceData EnsureItemInstance()
    {
        ItemInstanceData itemInstanceData = ResolveItemInstance();
        if (itemInstanceData != null)
        {
            return itemInstanceData;
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

        itemInstanceData = inventoryManager.CreateItemInstance(_initialItemKey);
        if (itemInstanceData == null)
        {
            return null;
        }

        _instanceId = itemInstanceData.InstanceId;
        _inventoryManager = inventoryManager;
        PropagateItemInstance();
        ActivateInstanceIfNeeded();
        return itemInstanceData;
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

    private ItemInstanceData ResolveItemInstance()
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
