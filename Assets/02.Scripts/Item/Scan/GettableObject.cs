using UnityEngine;

public class GettableObject : InteractableObject, IInitialItemSource
{
    [SerializeField] protected int _initialItemKey;
    public int InitialItemKey => _initialItemKey;
    
    public override void Interact()
    {
        if (!_isInteractActive)
        {
            Debug.Log($"{gameObject.name} : interaction is not active");
            return;
        }

        ItemInstance itemInstance = ItemInstance;
        if (itemInstance == null)
        {
            Debug.LogError($"[{nameof(GettableObject)}] {gameObject.name} has no bound ItemInstance.", this);
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[{nameof(GettableObject)}] {nameof(InventoryManager)}.Instance is null.", this);
            return;
        }

        InventoryManager.Instance.TryAddItem(itemInstance);
        OnInteractActivate();
        Destroy(gameObject);
    }
}
