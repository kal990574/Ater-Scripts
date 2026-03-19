using UnityEngine;

public class RewardInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemDataTable _itemDataTable;
    [SerializeField] private string _rewardItemId;

    public void Interact()
    {
        ItemData reward = _itemDataTable.GetItem(_rewardItemId);
        if (reward != null)
            InventoryManager.Instance.Inventory.AddItem(reward);

        Debug.Log($"{reward?.ItemName} 획득");

        // 획득 후 열쇠 오브젝트 비활성화
        gameObject.SetActive(false);
    }
}