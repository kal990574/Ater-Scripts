using UnityEngine;

[DisallowMultipleComponent]
public class RequireItemCondition : MonoBehaviour, IUseCondition
{
    [SerializeField] private int _requiredItemId = -1;

    public bool CanUse(InteractionContext context)
    {
        if (_requiredItemId < 0 || context?.Inventory == null)
        {
            return false;
        }

        RuntimeItemData currentHand = context.Hand;
        return currentHand != null && currentHand.ItemId == _requiredItemId;
    }
}
