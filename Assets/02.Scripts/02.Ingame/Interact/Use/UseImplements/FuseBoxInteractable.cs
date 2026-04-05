using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class FuseBoxInteractable : UsableObject
{
    [Header("References")]
    [SerializeField] private RuntimeView _runtimeView;
    [SerializeField] private GameObject _innerFuseObject;
    [SerializeField] private KeyPadInteractable _keyPadInteractable;

    [Header("Fuse Settings")]
    [SerializeField] private int _requiredFuseItemId = 11;
    [SerializeField] private bool _consumeRequiredItem = true;

    [Header("State Keys")]
    [SerializeField] private string _completedStateKey = "is_completed";

    [Header("Events")]
    [SerializeField] private UnityEvent _onInteractionFailed;
    [SerializeField] private UnityEvent _onCompleted;

    private void Awake()
    {
        if (_runtimeView == null)
        {
            _runtimeView = GetComponent<RuntimeView>();
        }
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (!ValidateConfiguration(out failureReason))
        {
            SetFailureResult(UseInteractResult.InvalidConfiguration);
            return false;
        }

        if (GetState(_completedStateKey))
        {
            SetFailureResult(UseInteractResult.AlreadyCompleted);
            failureReason = "The fuse box is already completed.";
            return false;
        }

        RuntimeItemData handItem = context?.Hand;
        if (handItem == null)
        {
            SetFailureResult(UseInteractResult.EmptyHand);
            failureReason = "A fuse is required, but the player's hand item is empty.";
            return false;
        }

        if (handItem.ItemId != _requiredFuseItemId)
        {
            SetFailureResult(UseInteractResult.NonRequireItem);
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
                SetFailureResult(UseInteractResult.ConsumeFailed);
                failureReason = "Failed to consume the fuse because HandAbility is missing.";
                Debug.LogError($"[{nameof(FuseBoxInteractable)}] {gameObject.name} could not consume the fuse because HandAbility is missing.", this);
                return false;
            }

            if (!context.HandAbility.TryConsumeCurrentHandItem())
            {
                SetFailureResult(UseInteractResult.ConsumeFailed);
                failureReason = $"Failed to consume the required fuse. requiredItemId={_requiredFuseItemId}";
                Debug.LogError($"[{nameof(FuseBoxInteractable)}] {gameObject.name} failed to consume the required fuse. requiredItemId={_requiredFuseItemId}", this);
                return false;
            }
        }

        if (_innerFuseObject != null)
        {
            _innerFuseObject.SetActive(true);
        }

        _keyPadInteractable?.Unlock();
        SetState(_completedStateKey, true);

        // TODO: Play fuse insertion SFX via SoundManager.

        Debug.Log($"[{nameof(FuseBoxInteractable)}] {gameObject.name} completed successfully. requiredItemId={_requiredFuseItemId}", this);
        _onCompleted?.Invoke();
        SetActivate(false);
        SetFailureResult(UseInteractResult.Success);
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

        if (string.IsNullOrWhiteSpace(_completedStateKey))
        {
            failureReason = "Completed state key is not configured.";
            return false;
        }

        if (ResolveRuntimeData()?.State == null)
        {
            failureReason = "RuntimeData.State is not available.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    private bool GetState(string key)
    {
        RuntimeData runtimeData = ResolveRuntimeData();
        if (runtimeData?.State == null || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return runtimeData.State.GetBool(key);
    }

    private void SetState(string key, bool value)
    {
        if (_runtimeView != null)
        {
            _runtimeView.SetBoolState(key, value);
            return;
        }

        RuntimeData runtimeData = ResolveRuntimeData();
        if (runtimeData?.State == null)
        {
            return;
        }

        runtimeData.State.SetBool(key, value);
        RefreshRuntimeView();
    }

    private RuntimeData ResolveRuntimeData()
    {
        if (_runtimeView != null && _runtimeView.RuntimeData != null)
        {
            return _runtimeView.RuntimeData;
        }

        return RuntimeData;
    }
}
