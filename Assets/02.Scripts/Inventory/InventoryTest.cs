using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTest : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            InventoryManager.Instance.Inventory.AddItem("test_item");
        }
    }
}