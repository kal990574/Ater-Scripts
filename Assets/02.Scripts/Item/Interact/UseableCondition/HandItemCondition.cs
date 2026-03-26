using UnityEngine;

[DisallowMultipleComponent]
public class HandItemCondition : MonoBehaviour, IUseCondition
{
    [SerializeField] private int _requiredItemId = -1;

    public bool CanUse(UseableObject useableObject)
    {
        if (_requiredItemId < 0 || InventoryManager.Instance == null)
        {
            return false;
        }

        ItemInstance currentHandItem = InventoryManager.Instance.CurrentHandItem;
        return currentHandItem != null && currentHandItem.ItemId == _requiredItemId;
    }
}
