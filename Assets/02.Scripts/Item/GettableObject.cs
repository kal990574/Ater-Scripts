using UnityEngine;

public class GettableObject : InteractableObject
{
    public override void Interact(UseContext context)
    {
        if (!_isInteractActive)
        {
            Debug.Log($"{gameObject.name} : interaction is not active");
            return;
        }

        ItemInstanceData itemInstanceData = ItemInstanceData;
        if (itemInstanceData == null)
        {
            Debug.LogError($"[{nameof(GettableObject)}] {gameObject.name} has no bound ItemInstance.", this);
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[{nameof(GettableObject)}] {nameof(InventoryManager)}.Instance is null.", this);
            return;
        }

        if (!InventoryManager.Instance.TryAddItem(itemInstanceData))
        {
            return;
        }

        PersistentSceneItem persistentSceneItem = GetComponentInParent<PersistentSceneItem>();
        persistentSceneItem?.MarkCollected(itemInstanceData.InstanceId);
        OnInteractActivate();
        Destroy(gameObject);
    }
}
