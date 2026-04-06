using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class KeyPadInteractable : StateInteractable
{
    [SerializeField] private KeyPadController _keyPadController;

    [Header("State Keys")]
    [SerializeField] private string _unlockStateKey = "is_unlocked";
    [SerializeField] private string _completedStateKey = "is_completed";

    [Header("Events")]
    [SerializeField] private UnityEvent _onPuzzleStarted;
    [SerializeField] private UnityEvent _onCompleted;

    protected override void OnAwake()
    {
        base.OnAwake();
        GetComponentCached(ref _keyPadController);
    }

    public bool Unlock()
    {
        if (!ValidateConfiguration(out string failureReason))
        {
            Debug.LogError($"[{nameof(KeyPadInteractable)}] {gameObject.name} unlock failed. reason={failureReason}", this);
            return false;
        }

        if (GetState(_unlockStateKey))
        {
            Debug.LogWarning($"[{nameof(KeyPadInteractable)}] {gameObject.name} unlock was requested, but it is already unlocked.", this);
            return false;
        }

        SetState(_unlockStateKey, true);
        SetActivate(true);
        Debug.Log($"[{nameof(KeyPadInteractable)}] {gameObject.name} unlocked and can now start the keypad puzzle.", this);
        return true;
    }

    public void HandlePuzzleSolved()
    {
        SetState(_completedStateKey, true);
        _onCompleted?.Invoke();
        SetActivate(false);
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (!ValidateConfiguration(out failureReason))
        {
            SetFailureResult(UseInteractResult.InvalidConfiguration);
            return false;
        }

        if (!GetState(_unlockStateKey))
        {
            SetFailureResult(UseInteractResult.Locked);
            failureReason = "The keypad is locked because the fuse box is not completed.";
            return false;
        }

        if (GetState(_completedStateKey))
        {
            SetFailureResult(UseInteractResult.AlreadyCompleted);
            failureReason = "The keypad puzzle is already completed.";
            return false;
        }

        if (_keyPadController.IsSolved)
        {
            SetFailureResult(UseInteractResult.AlreadyCompleted);
            failureReason = "The keypad controller is already solved.";
            return false;
        }

        if (_keyPadController.HasActivePuzzle)
        {
            SetFailureResult(UseInteractResult.PuzzleAlreadyRunning);
            failureReason = "The keypad puzzle is already running.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        _keyPadController.TryOpen(this);
        Debug.Log($"[{nameof(KeyPadInteractable)}] {gameObject.name} started the keypad puzzle.", this);
        _onPuzzleStarted?.Invoke();
        failureReason = string.Empty;
        SetFailureResult(UseInteractResult.Success);
        return true;
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }

    private bool ValidateConfiguration(out string failureReason)
    {
        if (_keyPadController == null)
        {
            failureReason = "KeyPadController reference is missing.";
            return false;
        }

        if (!ValidateStateKey(_unlockStateKey, "Unlock state key", out failureReason))
        {
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
