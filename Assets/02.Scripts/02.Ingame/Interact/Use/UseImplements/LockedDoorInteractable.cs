using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class LockedDoorInteractable : StateInteractable
{
    [SerializeField] private Animator _doorAnimator;

    [Header("Animation")]
    [SerializeField] private string _openAnimationStateName = "OpenDoor";

    [Header("State Keys")]
    [SerializeField] private string _unlockStateKey = "is_unlocked";
    [SerializeField] private string _openStateKey = "is_open";

    [Header("Events")]
    [SerializeField] private UnityEvent _onUnlocked;
    [SerializeField] private UnityEvent _onOpened;

    public bool IsUnlocked => GetState(_unlockStateKey);
    public bool IsOpen => GetState(_openStateKey);

    protected override bool IsAdditionalInteractRequirementSatisfied()
    {
        return IsUnlocked;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        GetComponentCached(ref _doorAnimator);
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (!ValidateConfiguration(out failureReason))
        {
            SetFailureResult(UseInteractResult.InvalidConfiguration);
            return false;
        }

        if (IsOpen)
        {
            SetFailureResult(UseInteractResult.AlreadyOpen);
            failureReason = "The door is already open.";
            return false;
        }

        if (!IsUnlocked)
        {
            SetFailureResult(UseInteractResult.Locked);
            failureReason = "The door is locked.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        OpenDoor();
        failureReason = string.Empty;
        SetFailureResult(UseInteractResult.Success);
        return true;
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }

    public virtual bool Unlock()
    {
        if (!ValidateConfiguration(out string failureReason))
        {
            Debug.LogError($"[{nameof(LockedDoorInteractable)}] {gameObject.name} unlock failed. reason={failureReason}", this);
            return false;
        }

        if (IsUnlocked)
        {
            Debug.LogWarning($"[{nameof(LockedDoorInteractable)}] {gameObject.name} unlock was requested, but it is already unlocked.", this);
            return false;
        }

        SetState(_unlockStateKey, true);
        RefreshInteractAvailability();
        Debug.Log($"[{nameof(LockedDoorInteractable)}] {gameObject.name} unlocked.", this);

        // TODO: Play unlock SFX via SoundManager.

        _onUnlocked?.Invoke();
        return true;
    }

    protected virtual void OpenDoor()
    {
        SetState(_openStateKey, true);

        if (_doorAnimator != null && !string.IsNullOrWhiteSpace(_openAnimationStateName))
        {
            _doorAnimator.Play(_openAnimationStateName);
        }
        else
        {
            Debug.LogWarning($"[{nameof(LockedDoorInteractable)}] {gameObject.name} is missing Animator or animation state name. animationState={_openAnimationStateName}", this);
        }

        // TODO: Play door open SFX via SoundManager.

        Debug.Log($"[{nameof(LockedDoorInteractable)}] {gameObject.name} opened successfully. animationState={_openAnimationStateName}", this);
        _onOpened?.Invoke();
        SetActivate(false);
    }

    protected virtual bool ValidateConfiguration(out string failureReason)
    {
        if (!ValidateStateKey(_unlockStateKey, "Unlock state key", out failureReason))
        {
            return false;
        }

        if (!ValidateStateKey(_openStateKey, "Open state key", out failureReason))
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
