using UnityEngine;

public class UseContext
{
    public GameObject User { get; }
    public GameObject TargetObject { get; }
    public UsableObject Target { get; }
    public InventoryManager Inventory { get; }
    public string HandItemInstanceId { get; }
    public InstanceData Hand => Inventory != null ? Inventory.GetItemInstance(HandItemInstanceId) : null;

    public UseContext(GameObject user, GameObject targetObject, UsableObject target, InventoryManager inventory, string handItemInstanceId)
    {
        User = user;
        TargetObject = targetObject;
        Target = target;
        Inventory = inventory;
        HandItemInstanceId = handItemInstanceId;
    }

    public static UseContext For(GameObject user, GameObject targetObject)
    {
        UsableObject usableObject = targetObject != null
            ? targetObject.GetComponent<UsableObject>()
            : null;

        InventoryManager inventory = InventoryManager.Instance;
        string handItemInstanceId = inventory != null ? inventory.CurrentHandItemInstanceId : null;
        return new UseContext(user, targetObject, usableObject, inventory, handItemInstanceId);
    }

    public static UseContext For(GameObject user, UsableObject target)
    {
        InventoryManager inventory = InventoryManager.Instance;
        string handItemInstanceId = inventory != null ? inventory.CurrentHandItemInstanceId : null;
        return new UseContext(user, target != null ? target.gameObject : null, target, inventory, handItemInstanceId);
    }
}
