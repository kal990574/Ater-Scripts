using UnityEngine;

public class InventoryTestHelper : MonoBehaviour
{
    [SerializeField] private int[] _testItemIds;

    private void Start()
    {
        foreach (int id in _testItemIds)
        {
            RuntimeItemData data = InventoryManager.Instance.CreateItemInstance(id);
            InventoryManager.Instance.TryAddItem(data);
        }
    }
}