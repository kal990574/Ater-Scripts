using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InventoryManager.Instance.ToggleInventory();
        }
    }
}