using UnityEngine;

public abstract class ItemInstanceBinderBase : MonoBehaviour
{
    private InventoryItemInstance _itemInstance;
    private InventoryManager _inventoryManager;

    public InventoryItemInstance ItemInstance => _itemInstance;
    public InventoryManager InventoryManager => _inventoryManager;

    public virtual void Bind(InventoryItemInstance itemInstance, InventoryManager inventoryManager)
    {
        _itemInstance = itemInstance;
        _inventoryManager = inventoryManager;
        RefreshView();
    }

    public void RefreshView()
    {
        MonoBehaviour[] behaviours = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IItemInstanceStateHandler handler)
            {
                handler.ApplyState(this);
            }
        }
    }
}
