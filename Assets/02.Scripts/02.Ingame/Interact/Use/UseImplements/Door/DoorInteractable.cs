using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class DoorInteractable : StateInteractable
{
    [TabGroup("Inspector", "DoorInteractable")]
    [SerializeField] private Animator _doorAnimator;

    [TabGroup("Inspector", "DoorInteractable")]
    [LabelText("Open State Name")]
    [SerializeField] private string _openAnimationStateName = "OpenDoor";

    [TabGroup("Inspector", "DoorInteractable")]
    [LabelText("Unlock")]
    [SerializeField] private string _unlockStateKey = "is_unlocked";

    [TabGroup("Inspector", "DoorInteractable")]
    [LabelText("Open")]
    [SerializeField] private string _openStateKey = "is_open";

    [TabGroup("Inspector", "DoorInteractable")]
    [LabelText("Unlocked Description")]
    [MultiLineProperty]
    [SerializeField] private string _hoverDescriptionUnlocked = "";

    [TabGroup("Inspector", "Events")]
    [LabelText("On Unlocked")]
    [SerializeField] private UnityEvent _onUnlocked;

    [TabGroup("Inspector", "Events")]
    [LabelText("On Opened")]
    [SerializeField] private UnityEvent _onOpened;

    private DoorInteractableConfig Config => new(_unlockStateKey, _openStateKey, _openAnimationStateName);
    private DoorInteractableStateSnapshot CurrentState => new(IsUnlocked, IsOpen);

    public bool IsUnlocked => GetState(_unlockStateKey);
    public bool IsOpen => GetState(_openStateKey);

    public override string HoverDescription
    {
        get
        {
            if (IsOpen) return string.Empty;
            if (IsUnlocked)
                return _hoverDescriptionUnlocked;

            return base.HoverDescription;
        }
    }

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
            SetFailureResult(EUseInteractResult.InvalidConfiguration);
            return false;
        }

        UseInteractionOutcome outcome = DoorInteractableStatePolicy.EvaluateOpen(CurrentState);
        if (!outcome.IsSuccess)
        {
            SetFailureResult(outcome.Result);
            failureReason = outcome.Reason;
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        OpenDoor();
        failureReason = string.Empty;
        SetFailureResult(EUseInteractResult.Success);
        return true;
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }
    
    public override bool Unlock()
    {
        if (!ValidateConfiguration(out string failureReason))
        {
            Debug.LogError($"[{nameof(DoorInteractable)}] {gameObject.name} unlock failed. reason={failureReason}", this);
            return false;
        }

        if (!DoorInteractableStatePolicy.CanUnlock(CurrentState, out failureReason))
        {
            Debug.LogWarning($"[{nameof(DoorInteractable)}] {gameObject.name} unlock was requested, but it was rejected. reason={failureReason}", this);
            return false;
        }

        ApplyUnlockState();
        RefreshInteractAvailability();
        Debug.Log($"[{nameof(DoorInteractable)}] {gameObject.name} unlocked.", this);

        PlayUnlockPresentation();
        return true;
    }

    public void ReactivateOpenedDoorInteraction()
    {
        if (!ValidateConfiguration(out string failureReason))
        {
            Debug.LogError($"[{nameof(DoorInteractable)}] {gameObject.name} reactivation failed. reason={failureReason}", this);
            return;
        }

        SetState(_openStateKey, false);
        SetActivate(true);

        Debug.Log($"[{nameof(DoorInteractable)}] {gameObject.name} interaction was reactivated.", this);
    }

    protected virtual void OpenDoor()
    {
        ApplyOpenState();
        PlayOpenPresentation();
    }

    protected virtual void ApplyUnlockState()
    {
        SetState(_unlockStateKey, true);
    }

    protected virtual void ApplyOpenState()
    {
        SetState(_openStateKey, true);
        SetActivate(false);
    }

    protected virtual void PlayUnlockPresentation()
    {
        _onUnlocked?.Invoke();
    }

    protected virtual void PlayOpenPresentation()
    {
        if (_doorAnimator != null && !string.IsNullOrWhiteSpace(_openAnimationStateName))
        {
            _doorAnimator.Play(_openAnimationStateName);
        }
        else
        {
            Debug.LogWarning($"[{nameof(DoorInteractable)}] {gameObject.name} is missing Animator or animation state name. animationState={_openAnimationStateName}", this);
        }
        
        Debug.Log($"[{nameof(DoorInteractable)}] {gameObject.name} opened successfully. animationState={_openAnimationStateName}", this);
        _onOpened?.Invoke();
    }

    protected override EInteractObjectEventType GetSuccessInteractEventType(InteractionContext context)
    {
        return EInteractObjectEventType.DoorOpen;
    }

    protected virtual bool ValidateConfiguration(out string failureReason)
    {
        return DoorInteractableValidator.Validate(Config, HasRuntimeState(), out failureReason);
    }
}
