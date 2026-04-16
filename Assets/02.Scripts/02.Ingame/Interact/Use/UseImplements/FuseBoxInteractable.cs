using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class FuseBoxInteractable : StateInteractable
{
    [TabGroup("Inspector", "FuseBoxInteractable")]
    [SerializeField] private GameObject _innerFuseObject;

    [TabGroup("Inspector", "FuseBoxInteractable")]
    [LabelText("Light On")]
    [SerializeField] private GameObject _fuseLightOn;

    [TabGroup("Inspector", "FuseBoxInteractable")]
    [LabelText("Light Off")]
    [SerializeField] private GameObject _fuseLightOff;

    [TabGroup("Inspector", "FuseBoxInteractable")]
    [MinValue(0)]
    [LabelText("Required Fuse Item ID")]
    [SerializeField] private int _requiredFuseItemId = 11;

    [TabGroup("Inspector", "FuseBoxInteractable")]
    [ToggleLeft]
    [LabelText("Consume Required Item")]
    [SerializeField] private bool _consumeRequiredItem = true;

    [TabGroup("Inspector", "FuseBoxInteractable")]
    [LabelText("Completed")]
    [SerializeField] private string _completedStateKey = "is_completed";

    [TabGroup("Inspector", "FuseBoxInteractable")]
    [LabelText("On Completed")]
    [SerializeField] private UnityEvent _onCompleted;

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (!ValidateConfiguration(out failureReason))
        {
            SetFailureResult(EUseInteractResult.InvalidConfiguration);
            return false;
        }

        if (GetState(_completedStateKey))
        {
            SetFailureResult(EUseInteractResult.AlreadyCompleted);
            failureReason = "The fuse box is already completed.";
            return false;
        }

        RuntimeItemData handItem = context?.Hand;
        if (handItem == null)
        {
            SetFailureResult(EUseInteractResult.EmptyHand);
            failureReason = "A fuse is required, but the player's hand item is empty.";
            return false;
        }

        if (handItem.ItemId != _requiredFuseItemId)
        {
            SetFailureResult(EUseInteractResult.NonRequireItem);
            failureReason = $"The equipped item does not match the required fuse. equippedItemId={handItem.ItemId}, requiredItemId={_requiredFuseItemId}";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        if (_consumeRequiredItem)
        {
            if (context?.HandAbility == null)
            {
                SetFailureResult(EUseInteractResult.ConsumeFailed);
                failureReason = "Failed to consume the fuse because HandAbility is missing.";
                Debug.LogError($"[{nameof(FuseBoxInteractable)}] {gameObject.name} could not consume the fuse because HandAbility is missing.", this);
                return false;
            }

            if (!context.HandAbility.TryConsumeCurrentHandItem())
            {
                SetFailureResult(EUseInteractResult.ConsumeFailed);
                failureReason = $"Failed to consume the required fuse. requiredItemId={_requiredFuseItemId}";
                Debug.LogError($"[{nameof(FuseBoxInteractable)}] {gameObject.name} failed to consume the required fuse. requiredItemId={_requiredFuseItemId}", this);
                return false;
            }
        }

        _innerFuseObject.SetActive(true);
        _fuseLightOn.SetActive(true);
        _fuseLightOff.SetActive(false);

        SetState(_completedStateKey, true);

        Debug.Log($"[{nameof(FuseBoxInteractable)}] {gameObject.name} completed successfully. requiredItemId={_requiredFuseItemId}", this);
        _onCompleted?.Invoke();
        SetActivate(false);
        SetFailureResult(EUseInteractResult.Success);
        failureReason = string.Empty;
        return true;
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }

    private bool ValidateConfiguration(out string failureReason)
    {
        if (_requiredFuseItemId < 0)
        {
            failureReason = "Required fuse item id is not configured.";
            return false;
        }

        if (!ValidateStateKey(_completedStateKey, "Completed state key", out failureReason))
        {
            return false;
        }

        if (!HasRuntimeState())
        {
            failureReason = "RuntimeData.State is not available.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }
}
