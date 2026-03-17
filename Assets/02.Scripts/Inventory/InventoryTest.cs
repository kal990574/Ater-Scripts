using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTest : MonoBehaviour
{
    [SerializeField] private ItemDataTable itemDataTable;

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            InventoryManager.Instance.Inventory.AddItem(itemDataTable.GetItem("Potion_Red"));
        }
    }
}