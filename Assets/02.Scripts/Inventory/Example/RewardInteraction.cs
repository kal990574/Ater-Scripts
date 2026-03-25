using UnityEngine;

public class RewardInteraction : MonoBehaviour, IInteractableUI, IInventoryItemViewInteractable, IInventoryItemViewStateHandler
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

    public void Interact(InventoryItemViewContext context)
    {
        if (context?.ItemInstance == null)
        {
            return;
        }

        if (context.ItemInstance.State.GetBool(RewardCollectedStateKey))
        {
            return;
        }

        if (!context.InventoryManager.TryAddItem(_rewardItemId))
        {
            return;
        }

        context.ItemInstance.State.SetBool(RewardCollectedStateKey, true);
        context.RefreshView();
    }

    public void ApplyState(InventoryItemViewContext context)
    {
        bool isCollected = context != null && context.ItemInstance != null && context.ItemInstance.State.GetBool(RewardCollectedStateKey);
        gameObject.SetActive(!isCollected);
    }
}
