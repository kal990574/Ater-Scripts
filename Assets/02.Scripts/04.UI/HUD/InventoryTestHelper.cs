using UnityEngine;

public class InventoryTestHelper : MonoBehaviour
{
    [SerializeField] private int[] _testItemIds;

    private void Start()
    {
        InventoryManager inventoryManager = InventoryManager.Instance;
        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;

        if (inventoryManager == null)
        {
            Debug.LogError($"[{nameof(InventoryTestHelper)}] InventoryManager is missing.", this);
            return;
        }

        if (runtimeInstanceManager == null)
        {
            Debug.LogError($"[{nameof(InventoryTestHelper)}] RuntimeInstanceManager is missing.", this);
            return;
        }

        for (int index = 0; index < _testItemIds.Length; index++)
        {
            int itemId = _testItemIds[index];
            RuntimeItemData runtimeItemData = runtimeInstanceManager.CreateItemInstance(itemId);
            if (runtimeItemData == null)
            {
                Debug.LogWarning($"[{nameof(InventoryTestHelper)}] Failed to create item. itemId={itemId}", this);
                continue;
            }

            inventoryManager.TryAddItem(runtimeItemData);
        }
    }
}