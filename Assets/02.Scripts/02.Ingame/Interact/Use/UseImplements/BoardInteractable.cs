using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class BoardInteractable : StateInteractable
{
    [Header("References")]
    [SerializeField] private Animator _animator;

    [Header("Animation")]
    [SerializeField] private string _openAnimationStateName = "BoardOpen";

    [Header("State Keys")]
    [SerializeField] private string _openStateKey = "is_open";

    [Header("Events")]
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
            SetFailureResult(UseInteractResult.InvalidConfiguration);
            return false;
        }

        if (GetState(_openStateKey))
        {
            SetFailureResult(UseInteractResult.AlreadyOpen);
            failureReason = "The board is already open.";
            return false;
        }

        if (!_scannableObject.IsProgressComplete)
        {
            SetFailureResult(UseInteractResult.NotScanned);
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
