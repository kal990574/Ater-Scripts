using UnityEngine;

public class RewardInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemDataTable _itemDataTable;
    [SerializeField] private int _rewardItemId;

    public void Interact()
    {
        ItemData reward = _itemDataTable.GetItem(_rewardItemId);
        if (reward != null)
            InventoryManager.Instance.AddItem(reward);

        Debug.Log($"{reward?.ItemName} 획득");

        gameObject.SetActive(false);
    }
}