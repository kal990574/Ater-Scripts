using UnityEngine;

public class InventoryInputController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InventoryManager.Instance.ToggleInventory();
        }
    }
}