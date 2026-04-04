using UnityEngine;

public class UseContext
{
    public GameObject User { get; }
    public GameObject TargetObject { get; }
    public UsableObject Target { get; }
    public InventoryManager Inventory { get; }
    public PlayerInventoryAbility InventoryAbility { get; }
    public string HandItemInstanceId { get; }
    public RuntimeItemData Hand { get; }

    public UseContext(
        GameObject user,
        GameObject targetObject,
        UsableObject target,
        InventoryManager inventory,
        PlayerInventoryAbility inventoryAbility,
        string handItemInstanceId,
        RuntimeItemData hand)
    {
        User = user;
        TargetObject = targetObject;
        Target = target;
        Inventory = inventory;
        InventoryAbility = inventoryAbility;
        HandItemInstanceId = handItemInstanceId;
        Hand = hand;
    }

    public static UseContext For(GameObject user, GameObject targetObject)
    {
        UsableObject usableObject = targetObject != null
            ? targetObject.GetComponent<UsableObject>()
            : null;

        return Create(user, usableObject, targetObject);
    }

    public static UseContext For(GameObject user, UsableObject target)
    {
        return Create(user, target, target != null ? target.gameObject : null);
    }

    private static UseContext Create(GameObject user, UsableObject target, GameObject targetObject)
    {
        InventoryManager inventory = InventoryManager.Instance;
        PlayerInventoryAbility inventoryAbility = ResolveInventoryAbility(user);

        string handItemInstanceId = inventoryAbility != null
            ? inventoryAbility.CurrentHandItemInstanceId
            : null;

        RuntimeItemData hand = null;
        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
        if (runtimeInstanceManager != null && string.IsNullOrEmpty(handItemInstanceId) == false)
        {
            hand = runtimeInstanceManager.GetItemInstance(handItemInstanceId);
        }

        return new UseContext(
            user,
            targetObject,
            target,
            inventory,
            inventoryAbility,
            handItemInstanceId,
            hand);
    }

    private static PlayerInventoryAbility ResolveInventoryAbility(GameObject user)
    {
        if (user == null)
        {
            return null;
        }

        PlayerInventoryAbility inventoryAbility = user.GetComponent<PlayerInventoryAbility>();
        if (inventoryAbility != null)
        {
            return inventoryAbility;
        }

        return user.GetComponentInChildren<PlayerInventoryAbility>();
    }
}