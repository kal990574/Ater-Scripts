using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class BoardInteractable : StateInteractable
{
    [TabGroup("Inspector", "BoardInteractable")]
    [SerializeField] private Animator _animator;

    [TabGroup("Inspector", "BoardInteractable")]
    [LabelText("Open State Name")]
    [SerializeField] private string _openAnimationStateName = "BoardOpen";

    [TabGroup("Inspector", "BoardInteractable")]
    [LabelText("Open")]
    [SerializeField] private string _openStateKey = "is_open";

    [TabGroup("Inspector", "BoardInteractable")]
    [LabelText("On Opened")]
    [SerializeField] private UnityEvent _onOpened;

    protected override void OnAwake()
    {
        base.OnAwake();
        GetComponentCached(ref _animator);
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (!ValidateConfiguration(out failureReason))
        {
            SetFailureResult(EUseInteractResult.InvalidConfiguration);
            return false;
        }

        if (GetState(_openStateKey))
        {
            SetFailureResult(EUseInteractResult.AlreadyOpen);
            failureReason = "The board is already open.";
            return false;
        }

        if (!_scannableObject.IsProgressComplete)
        {
            SetFailureResult(EUseInteractResult.NotScanned);
            failureReason = "The board cannot be opened before scan completion.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        SetState(_openStateKey, true);

        if (_animator != null && !string.IsNullOrWhiteSpace(_openAnimationStateName))
        {
            _animator.Play(_openAnimationStateName);
        }
        else
        {
            Debug.LogWarning($"[{nameof(BoardInteractable)}] {gameObject.name} is missing Animator or animation state name. animationState={_openAnimationStateName}", this);
        }

        // TODO: Play board open SFX via SoundManager.

        Debug.Log($"[{nameof(BoardInteractable)}] {gameObject.name} opened successfully. animationState={_openAnimationStateName}", this);
        _onOpened?.Invoke();
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
        if (_scannableObject == null)
        {
            failureReason = "ScannableObject reference is missing.";
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
