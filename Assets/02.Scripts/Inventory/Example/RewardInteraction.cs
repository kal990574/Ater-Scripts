using UnityEngine;

public class RewardInteraction : MonoBehaviour, IInteractableUI
{
    [SerializeField] private ItemDataTable _itemDataTable;
    [SerializeField] private int _rewardItemId;

    public void Interact()
    {
        if (!InventoryManager.Instance.TryAddItem(_rewardItemId))
        {
            return;
        }

        gameObject.SetActive(false);
    }
}