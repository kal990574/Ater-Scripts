using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class LockedDoorInteractable : UsableObject
{
    [Header("References")]
    [SerializeField] private RuntimeView _runtimeView;
    [SerializeField] private Animator _doorAnimator;

    [Header("Animation")]
    [SerializeField] private string _openAnimationStateName = "OpenDoor";

    [Header("State Keys")]
    [SerializeField] private string _unlockStateKey = "is_unlocked";
    [SerializeField] private string _openStateKey = "is_open";

    [Header("Events")]
    [SerializeField] protected UnityEvent _onInteractionFailed;
    [SerializeField] private UnityEvent _onUnlocked;
    [SerializeField] private UnityEvent _onOpened;

    public bool IsUnlocked => GetState(_unlockStateKey);
    public bool IsOpen => GetState(_openStateKey);

    protected override bool IsAdditionalInteractRequirementSatisfied()
    {
        return IsUnlocked;
    }

    private void Awake()
    {
        if (_runtimeView == null)
        {
            _runtimeView = GetComponent<RuntimeView>();
        }

        if (_doorAnimator == null)
        {
            _doorAnimator = GetComponent<Animator>();
        }
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
        if (string.IsNullOrWhiteSpace(_unlockStateKey))
        {
            failureReason = "Unlock state key is not configured.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(_openStateKey))
        {
            failureReason = "Open state key is not configured.";
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

    protected bool GetState(string key)
    {
        RuntimeData runtimeData = ResolveRuntimeData();
        if (runtimeData?.State == null || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return runtimeData.State.GetBool(key);
    }

    protected void SetState(string key, bool value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

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

    protected RuntimeData ResolveRuntimeData()
    {
        if (_runtimeView != null && _runtimeView.RuntimeData != null)
        {
            return _runtimeView.RuntimeData;
        }

        return RuntimeData;
    }
}
