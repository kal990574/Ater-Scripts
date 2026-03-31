using Unity.VisualScripting;
using UnityEngine;

public class InvenTester : MonoBehaviour
{
    [SerializeField] private InventoryManager _inven;

    [ContextMenu("add")]
    void AddItem()
    {
        _inven.TryAddItem(_inven.CreateItemInstance(3));
        //
    }

    [ContextMenu("toggle")]
    public void Toggle()
    {
        _inven.ToggleInventory();
    }
}
