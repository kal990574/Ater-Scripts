using UnityEngine;

public class UseContext
{
    public GameObject User { get; }
    public GameObject TargetObject { get; }
    public UsableObject Target { get; }
    public InventoryManager Inventory { get; }
    public ItemInstance HandItem { get; }

    public UseContext(GameObject user, GameObject targetObject, UsableObject target, InventoryManager inventory, ItemInstance handItem)
    {
        User = user;
        TargetObject = targetObject;
        Target = target;
        Inventory = inventory;
        HandItem = handItem;
    }

    public static UseContext For(GameObject user, GameObject targetObject)
    {
        UsableObject usableObject = targetObject != null
            ? targetObject.GetComponent<UsableObject>()
            : null;

        InventoryManager inventory = InventoryManager.Instance;
        ItemInstance handItem = inventory != null ? inventory.CurrentHandItem : null;
        return new UseContext(user, targetObject, usableObject, inventory, handItem);
    }

    public static UseContext For(GameObject user, UsableObject target)
    {
        InventoryManager inventory = InventoryManager.Instance;
        ItemInstance handItem = inventory != null ? inventory.CurrentHandItem : null;
        return new UseContext(user, target != null ? target.gameObject : null, target, inventory, handItem);
    }
}
