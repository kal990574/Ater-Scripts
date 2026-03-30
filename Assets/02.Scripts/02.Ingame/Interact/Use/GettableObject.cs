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
        
        if (TryGetHub(out GameEventHub hub) == false)
        {
            Debug.LogWarning("[PickupEventEmitter] GameEventHub가 존재하지 않습니다.");
            return;
        }
        
        GameEventContext eventContext = CreateContext();
        ItemGetEvent gameEvent = new ItemGetEvent(
             eventContext, 
            runtimeItemData.InstanceId, 
            runtimeItemData.ItemId);

        hub.Publish(in gameEvent);
        OnInteractActivate();
        Destroy(gameObject);
    }
}
