using UnityEngine;

public class RewardInteraction : MonoBehaviour, IInteractableUI, IUIViewItemInteractable, IItemInstanceStateHandler
{
    private const string RewardCollectedStateKey = "reward_collected";

    [SerializeField] private int _rewardItemId;

    public void Interact()
    {
        if (!InventoryManager.Instance.TryAddItem(_rewardItemId))
        {
            return;
        }

        gameObject.SetActive(false);
    }

    public void Interact(UIViewItemBinder binder)
    {
        if (binder?.ItemInstance == null)
        {
            return;
        }

        if (binder.ItemInstance.State.GetBool(RewardCollectedStateKey))
        {
            return;
        }

        if (!binder.InventoryManager.TryAddItem(_rewardItemId))
        {
            return;
        }

        binder.ItemInstance.State.SetBool(RewardCollectedStateKey, true);
        binder.RefreshView();
    }

    public void ApplyState(ItemInstanceBinderBase binder)
    {
        bool isCollected = binder != null && binder.ItemInstance != null && binder.ItemInstance.State.GetBool(RewardCollectedStateKey);
        gameObject.SetActive(!isCollected);
    }
}
