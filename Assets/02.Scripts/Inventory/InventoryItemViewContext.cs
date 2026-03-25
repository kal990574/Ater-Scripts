using UnityEngine;

public class InventoryItemViewContext : MonoBehaviour
{
    private InventoryItemInstance _itemInstance;
    private InventoryManager _inventoryManager;

    public InventoryItemInstance ItemInstance => _itemInstance;
    public InventoryManager InventoryManager => _inventoryManager;

    public void Bind(InventoryItemInstance itemInstance, InventoryManager inventoryManager)
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
            if (behaviour is IInventoryItemViewStateHandler handler)
            {
                handler.ApplyState(this);
            }
        }
    }
}
