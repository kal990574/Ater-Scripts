using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTest : MonoBehaviour
{
    [SerializeField] private ItemDataTable _itemDataTable;

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            InventoryManager.Instance.Inventory.AddItem(_itemDataTable.GetItem("Potion_Red"));
        }
    }
}