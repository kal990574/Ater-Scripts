using UnityEngine;

public class InstanceView : MonoBehaviour, IItemInstance
{
    [SerializeField]private ItemInstance _itemInstance;
    private InventoryManager _inventoryManager;

    public ItemInstance ItemInstance => _itemInstance;
    public InventoryManager InventoryManager => _inventoryManager;

    public virtual void Bind(ItemInstance itemInstance, InventoryManager inventoryManager)
    {
        _itemInstance = itemInstance;
        _inventoryManager = inventoryManager;
        RefreshView();
    }

    public void RefreshView()
    {
        IBindApplier binder = GetComponentInChildren<IBindApplier>();
        if (binder == null)
        {
            return;
        }
        
        binder.ApplyState(this);
    }
}
