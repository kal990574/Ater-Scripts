using UnityEngine;

public class InvenTester : MonoBehaviour
{
    [SerializeField] private InventoryManager _inven;

    [ContextMenu("add")]
    private void AddItem()
    {
        if (_inven == null)
        {
            Debug.LogError($"[{nameof(InvenTester)}] InventoryManager is missing.", this);
            return;
        }

        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
        if (runtimeInstanceManager == null)
        {
            Debug.LogError($"[{nameof(InvenTester)}] RuntimeInstanceManager is missing.", this);
            return;
        }

        RuntimeItemData runtimeItemData = runtimeInstanceManager.CreateItemInstance(3);
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(InvenTester)}] Failed to create test item instance.", this);
            return;
        }

        _inven.TryAddItem(runtimeItemData);
    }

    [ContextMenu("toggle")]
    public void Toggle()
    {
        if (_inven == null)
        {
            return;
        }

        _inven.ToggleInventory();
    }
}