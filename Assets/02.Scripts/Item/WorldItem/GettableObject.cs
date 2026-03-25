using Unity.VisualScripting;
using UnityEngine;

public class GettableObject : InteractableObject
{
    public override void Interact()
    {
        if (!_isInteractActive)
        {
            Debug.Log($"{gameObject.name} : interaction is not active");
            return;
        }

        //현재 serializefield라 null체킹이 안됨
        ItemInstance itemInstance = ItemInstance != null ? ItemInstance : InventoryManager.Instance.CreateItemInstance(_initialItemKey);
        if (itemInstance == null)
        {
            return;
        }
        
        InventoryManager.Instance.TryAddItem(itemInstance);
        OnInteractActivate();
        Destroy(gameObject);
    }
    
}
