using UnityEngine;

[DisallowMultipleComponent]
public class ConsumeCurrentHandItemAction : MonoBehaviour, IUseAction
{
    public void Execute(UseContext context)
    {
        if (context?.Inventory == null)
        {
            Debug.LogError($"[{nameof(ConsumeCurrentHandItemAction)}] Inventory is missing.", this);
            return;
        }

        if (!context.Inventory.RemoveCurrentHandItem())
        {
            Debug.LogWarning($"[{nameof(ConsumeCurrentHandItemAction)}] No current hand item to consume.", this);
        }
    }
}
