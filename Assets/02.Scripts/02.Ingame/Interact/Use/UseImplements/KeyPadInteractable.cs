using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class KeyPadInteractable : StateInteractable
{
    [SerializeField] private KeyPadController keyPadController;

    [Header("State Keys")]
    [SerializeField] private string _unlockStateKey = "is_unlocked";
    [SerializeField] private string _completedStateKey = "is_completed";

    [Header("Events")]
    [SerializeField] private UnityEvent _onPuzzleStarted;
    [SerializeField] private UnityEvent _onCompleted;

    protected override void OnAwake()
    {
        base.OnAwake();
        GetComponentCached(ref keyPadController);
    }
    
    public override bool Unlock()
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
            SetFailureResult(EUseInteractResult.InvalidConfiguration);
            return false;
        }

        if (!GetState(_unlockStateKey))
        {
            SetFailureResult(EUseInteractResult.Locked);
            failureReason = "The keypad is locked because the fuse box is not completed.";
            return false;
        }

        if (GetState(_completedStateKey))
        {
            SetFailureResult(EUseInteractResult.AlreadyCompleted);
            failureReason = "The keypad puzzle is already completed.";
            return false;
        }

        if (keyPadController.IsSolved)
        {
            SetFailureResult(EUseInteractResult.AlreadyCompleted);
            failureReason = "The keypad controller is already solved.";
            return false;
        }

        if (keyPadController.HasActivePuzzle)
        {
            SetFailureResult(EUseInteractResult.PuzzleAlreadyRunning);
            failureReason = "The keypad puzzle is already running.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        keyPadController.TryOpen(this);
        Debug.Log($"[{nameof(KeyPadInteractable)}] {gameObject.name} started the keypad puzzle.", this);
        _onPuzzleStarted?.Invoke();
        failureReason = string.Empty;
        SetFailureResult(EUseInteractResult.Success);
        return true;
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }

    private bool ValidateConfiguration(out string failureReason)
    {
        if (keyPadController == null)
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
