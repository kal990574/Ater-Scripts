using System;
using UnityEngine;

public class RuntimeView : MonoBehaviour, IRuntimeView
{
    [SerializeField] private string _instanceId;
    [SerializeField] private int _initialItemKey = -1;
    [SerializeField] private bool _InstanceOnInit = false;
    
    private InventoryManager _inventoryManager;

    public string InstanceId => _instanceId;
    public RuntimeItemData RuntimeItemData => ResolveItemInstance();
    public InventoryManager InventoryManager => _inventoryManager;
    public int InitialItemKey => _initialItemKey;

    private void Start()
    {
        if (!_InstanceOnInit)
        {
            return;
        }

        if (ResolveItemInstance() != null)
        {
            ActivateInstanceIfNeeded();
            return;
        }

        if (InventoryManager.Instance != null)
        {
            RuntimeItemData runtimeItemData = InventoryManager.Instance.CreateItemInstance(_initialItemKey);
            Bind(runtimeItemData != null ? runtimeItemData.InstanceId : null, InventoryManager.Instance);
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

    public RuntimeItemData EnsureItemInstance()
    {
        RuntimeItemData runtimeItemData = ResolveItemInstance();
        if (runtimeItemData != null)
        {
            return runtimeItemData;
        }

        InventoryManager inventoryManager = ResolveInventoryManager();
        if (inventoryManager == null)
        {
            Debug.LogError($"[{nameof(RuntimeView)}] {nameof(InventoryManager)}.Instance is null.", this);
            return null;
        }

        if (_initialItemKey < 0)
        {
            Debug.LogError($"[{nameof(RuntimeView)}] Initial item key is missing or invalid.", this);
            return null;
        }

        runtimeItemData = inventoryManager.CreateItemInstance(_initialItemKey);
        if (runtimeItemData == null)
        {
            return null;
        }

        _instanceId = runtimeItemData.InstanceId;
        _inventoryManager = inventoryManager;
        PropagateItemInstance();
        ActivateInstanceIfNeeded();
        return runtimeItemData;
    }

    public void RefreshView()
    {
        EnsureItemInstance();

        IStateApplier binder = GetComponentInChildren<IStateApplier>();
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
        INeedItemInstance[] itemBindables = GetComponentsInChildren<INeedItemInstance>(true);
        foreach (INeedItemInstance itemBindable in itemBindables)
        {
            itemBindable?.SetInstance(this);
        }
    }

    private RuntimeItemData ResolveItemInstance()
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
