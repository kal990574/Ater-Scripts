using System;
using UnityEngine;

public class InstanceView : MonoBehaviour, IItemInstance
{
    [SerializeField] private ItemInstance _itemInstance;
    [SerializeField] private int _initialItemKey = -1;
    [SerializeField] private bool _initInstnace = false;
    private InventoryManager _inventoryManager;

    public ItemInstance ItemInstance => _itemInstance;
    public InventoryManager InventoryManager => _inventoryManager;
    public int InitialItemKey => _initialItemKey;

    private void Start()
    {
        if (_initInstnace && InventoryManager.Instance != null)
        {
            _itemInstance = InventoryManager.Instance.CreateItemInstance(_initialItemKey);
            Bind(_itemInstance, InventoryManager.Instance);
        }
    }

    public virtual void Bind(ItemInstance itemInstance, InventoryManager inventoryManager)
    {
        _itemInstance = itemInstance;
        _inventoryManager = inventoryManager;
        PropagateItemInstance();
        RefreshView();
    }

    public ItemInstance EnsureItemInstance()
    {
        if (_itemInstance != null)
        {
            return _itemInstance;
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

        _itemInstance = inventoryManager.CreateItemInstance(_initialItemKey);
        if (_itemInstance == null)
        {
            return null;
        }

        _inventoryManager = inventoryManager;
        PropagateItemInstance();
        return _itemInstance;
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

    
}
