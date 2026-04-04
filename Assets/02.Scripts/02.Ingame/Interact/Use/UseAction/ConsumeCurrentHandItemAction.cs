using UnityEngine;

[DisallowMultipleComponent]
public class ConsumeCurrentHandItemAction : MonoBehaviour, IUseAction
{
    public void Execute(UseContext context)
    {
        if (context == null)
        {
            Debug.LogError($"[{nameof(ConsumeCurrentHandItemAction)}] Context is missing.", this);
            return;
        }

        if (context.InventoryAbility == null)
        {
            Debug.LogError($"[{nameof(ConsumeCurrentHandItemAction)}] Inventory ability is missing.", this);
            return;
        }

        if (context.InventoryAbility.TryConsumeCurrentHandItem() == false)
        {
            Debug.LogWarning($"[{nameof(ConsumeCurrentHandItemAction)}] No current hand item to consume.", this);
        }
    }
}