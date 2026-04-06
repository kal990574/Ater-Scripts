using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class KeyDoorInteractable : LockedDoorInteractable
{
    [Header("Key Settings")]
    [SerializeField] private int _requiredKeyItemId = -1;
    [SerializeField] private bool _consumeRequiredItemOnUnlock = true;

    protected override bool IsAdditionalInteractRequirementSatisfied()
    {
        return true;
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (!IsUnlocked)
        {
            return CanUnlock(context, out failureReason);
        }

        return base.CanUse(context, out failureReason);
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        if (!IsUnlocked)
        {
            return UnlockDoor(context, out failureReason);
        }

        return base.OnUse(context, out failureReason);
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }

    private bool CanUnlock(InteractionContext context, out string failureReason)
    {
        RuntimeItemData handItem = context?.Hand;
        if (handItem == null)
        {
            SetFailureResult(UseInteractResult.EmptyHand);
            failureReason = "A key is required, but the player's hand item is empty.";
            return false;
        }

        if (handItem.ItemId != _requiredKeyItemId)
        {
            SetFailureResult(UseInteractResult.NonRequireItem);
            failureReason = $"The equipped item does not match the required key. equippedItemId={handItem.ItemId}, requiredItemId={_requiredKeyItemId}";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    private bool UnlockDoor(InteractionContext context, out string failureReason)
    {
        if (_consumeRequiredItemOnUnlock)
        {
            if (context?.HandAbility == null)
            {
                SetFailureResult(UseInteractResult.ConsumeFailed);
                failureReason = "Failed to consume the key because HandAbility is missing.";
                Debug.LogError($"[{nameof(KeyDoorInteractable)}] {gameObject.name} could not consume the key because HandAbility is missing.", this);
                return false;
            }

            if (!context.HandAbility.TryConsumeCurrentHandItem())
            {
                SetFailureResult(UseInteractResult.ConsumeFailed);
                failureReason = $"Failed to consume the required key. requiredItemId={_requiredKeyItemId}";
                Debug.LogError($"[{nameof(KeyDoorInteractable)}] {gameObject.name} failed to consume the required key. requiredItemId={_requiredKeyItemId}", this);
                return false;
            }
        }

        if (!Unlock())
        {
            SetFailureResult(UseInteractResult.InvalidConfiguration);
            failureReason = "Door unlock request failed.";
            return false;
        }
        
        failureReason = string.Empty;
        SetFailureResult(UseInteractResult.Success);
        return true;
    }

    protected override bool ValidateConfiguration(out string failureReason)
    {
        if (_requiredKeyItemId < 0)
        {
            failureReason = "Required key item id is not configured.";
            return false;
        }

        return base.ValidateConfiguration(out failureReason);
    }
}
