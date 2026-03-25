using UnityEngine;

//아이템 인스턴스를 적용
public abstract class ItemBinderBase : MonoBehaviour
{
    private ItemInstance _itemInstance;
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
        MonoBehaviour[] behaviours = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IBindApplier handler)
            {
                handler.ApplyState(this);
            }
        }
    }
}
