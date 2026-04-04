using UnityEngine;

public class InteractionContext
{
    public GameObject User { get; }
    public GameObject TargetObject { get; }
    public UsableObject Target { get; }
    public InventoryManager Inventory { get; }
    public PlayerHandAbility HandAbility { get; }
    public string HandItemInstanceId { get; }
    public RuntimeItemData Hand { get; }

    public InteractionContext(
        GameObject user,
        GameObject targetObject,
        UsableObject target,
        InventoryManager inventory,
        PlayerHandAbility handAbility,
        string handItemInstanceId,
        RuntimeItemData hand)
    {
        User = user;
        TargetObject = targetObject;
        Target = target;
        Inventory = inventory;
        HandAbility = handAbility;
        HandItemInstanceId = handItemInstanceId;
        Hand = hand;
    }

    public static InteractionContext For(GameObject user, GameObject targetObject)
    {
        UsableObject usableObject = targetObject != null
            ? targetObject.GetComponent<UsableObject>()
            : null;

        return Create(user, usableObject, targetObject);
    }

    public static InteractionContext For(GameObject user, UsableObject target)
    {
        return Create(user, target, target != null ? target.gameObject : null);
    }

    private static InteractionContext Create(GameObject user, UsableObject target, GameObject targetObject)
    {
        InventoryManager inventory = InventoryManager.Instance;
        PlayerHandAbility handAbility = ResolveInventoryAbility(user);

        string handItemInstanceId = handAbility != null
            ? handAbility.CurrentHandItemInstanceId
            : null;

        RuntimeItemData hand = null;
        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
        if (runtimeInstanceManager != null && string.IsNullOrEmpty(handItemInstanceId) == false)
        {
            hand = runtimeInstanceManager.GetItemInstance(handItemInstanceId);
        }

        return new InteractionContext(
            user,
            targetObject,
            target,
            inventory,
            handAbility,
            handItemInstanceId,
            hand);
    }

    private static PlayerHandAbility ResolveInventoryAbility(GameObject user)
    {
        if (user == null)
        {
            return null;
        }

        PlayerHandAbility handAbility = user.GetComponent<PlayerHandAbility>();
        if (handAbility != null)
        {
            return handAbility;
        }

        return user.GetComponentInChildren<PlayerHandAbility>();
    }
}