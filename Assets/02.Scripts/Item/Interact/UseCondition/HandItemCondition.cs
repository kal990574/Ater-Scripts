using UnityEngine;

[DisallowMultipleComponent]
public class HandItemCondition : MonoBehaviour, IUseCondition
{
    [SerializeField] private int _requiredItemId = -1;

    public bool CanUse(UseContext context)
    {
        if (_requiredItemId < 0 || context?.Inventory == null)
        {
            return false;
        }

        ItemInstance currentHandItem = context.HandItem;
        return currentHandItem != null && currentHandItem.ItemId == _requiredItemId;
    }
}
