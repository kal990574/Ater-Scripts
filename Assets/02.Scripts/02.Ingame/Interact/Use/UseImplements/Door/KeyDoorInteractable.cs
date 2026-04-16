using Sirenix.OdinInspector;
using UnityEngine;

[DisallowMultipleComponent]
public class KeyDoorInteractable : DoorInteractable
{
    [TabGroup("Inspector", "KeyDoorInteractable")]
    [MinValue(0)]
    [LabelText("Required Key Item ID")]
    [SerializeField] private int _requiredKeyItemId = -1;

    [TabGroup("Inspector", "KeyDoorInteractable")]
    [ToggleLeft]
    [LabelText("Consume On Unlock")]
    [SerializeField] private bool _consumeRequiredItemOnUnlock = true;
    private bool _unlockedDuringCurrentUse;

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
        _unlockedDuringCurrentUse = false;

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

    protected override EInteractObjectEventType GetSuccessInteractEventType(InteractionContext context)
    {
        if (_unlockedDuringCurrentUse)
        {
            _unlockedDuringCurrentUse = false;
            return EInteractObjectEventType.Unlock;
        }

        return base.GetSuccessInteractEventType(context);
    }

    protected override void OnUseSucceeded(InteractionContext context)
    {
        if (_unlockedDuringCurrentUse)
        {
            PublishObjectInteracted(GetSuccessInteractEventType(context));
            return;
        }

        base.OnUseSucceeded(context);
    }

    private bool CanUnlock(InteractionContext context, out string failureReason)
    {
        RuntimeItemData handItem = context?.Hand;
        if (handItem == null)
        {
            SetFailureResult(EUseInteractResult.EmptyHand);
            failureReason = "A key is required, but the player's hand item is empty.";
            return false;
        }

        if (handItem.ItemId != _requiredKeyItemId)
        {
            SetFailureResult(EUseInteractResult.NonRequireItem);
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
                SetFailureResult(EUseInteractResult.ConsumeFailed);
                failureReason = "Failed to consume the key because HandAbility is missing.";
                Debug.LogError($"[{nameof(KeyDoorInteractable)}] {gameObject.name} could not consume the key because HandAbility is missing.", this);
                return false;
            }

            if (!context.HandAbility.TryConsumeCurrentHandItem())
            {
                SetFailureResult(EUseInteractResult.ConsumeFailed);
                failureReason = $"Failed to consume the required key. requiredItemId={_requiredKeyItemId}";
                Debug.LogError($"[{nameof(KeyDoorInteractable)}] {gameObject.name} failed to consume the required key. requiredItemId={_requiredKeyItemId}", this);
                return false;
            }
        }

        if (!Unlock())
        {
            SetFailureResult(EUseInteractResult.InvalidConfiguration);
            failureReason = "Door unlock request failed.";
            return false;
        }

        _unlockedDuringCurrentUse = true;
        failureReason = string.Empty;
        SetFailureResult(EUseInteractResult.Success);
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
