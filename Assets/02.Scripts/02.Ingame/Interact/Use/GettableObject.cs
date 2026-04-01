using UnityEngine;

public class GettableObject : Interactable
{
    public override void Interact(UseContext context)
    {
        if (!_isInteractActive)
        {
            Debug.Log($"{gameObject.name} : interaction is not active");
            return;
        }

        RuntimeItemData runtimeItemData = RuntimeItemData;
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(GettableObject)}] {gameObject.name} has no bound ItemInstance.", this);
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[{nameof(GettableObject)}] {nameof(InventoryManager)}.Instance is null.", this);
            return;
        }

        InventoryManager.Instance.TryAddItem(runtimeItemData);
        OnInteractActivate();
        Destroy(gameObject);
    }
}
