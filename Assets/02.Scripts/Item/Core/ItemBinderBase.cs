using UnityEngine;

//아이템 인스턴스의 스테이트 변수들을 적용
public abstract class ItemBinderBase : MonoBehaviour
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
