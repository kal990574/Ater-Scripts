using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PuzzleInteractable : StateInteractable
{
    [TabGroup("Inspector", "PuzzleInteractable")]
    [Required]
    [SerializeField] private PuzzleControllerBase PuzzleBase;

    [TabGroup("Inspector", "PuzzleInteractable")]
    [LabelText("Unlock")]
    [SerializeField] private string _unlockStateKey = "is_unlocked";

    [TabGroup("Inspector", "PuzzleInteractable")]
    [LabelText("Completed")]
    [SerializeField] private string _completedStateKey = "is_completed";

    [TabGroup("Inspector", "PuzzleInteractable")]
    [ToggleLeft]
    [LabelText("Require Unlock")]
    [SerializeField] private bool _haveToUnlock = false;
    
    [TabGroup("Inspector", "PuzzleInteractable")]
    [LabelText("On Puzzle Started")]
    [SerializeField] private UnityEvent _onPuzzleStarted;

    [TabGroup("Inspector", "PuzzleInteractable")]
    [LabelText("On Completed")]
    [SerializeField] private UnityEvent _onCompleted;

    protected override void OnAwake()
    {
        base.OnAwake();
        GetComponentCached(ref PuzzleBase);
    }

    public override bool Unlock()
    {
        if (!ValidateConfiguration(out string failureReason))
        {
            Debug.LogError($"[{nameof(PuzzleInteractable)}] {gameObject.name} unlock failed. reason={failureReason}", this);
            return false;
        }

        if (GetState(_unlockStateKey))
        {
            Debug.LogWarning($"[{nameof(PuzzleInteractable)}] {gameObject.name} unlock was requested, but it is already unlocked.", this);
            return false;
        }

        SetState(_unlockStateKey, true);
        SetActivate(true);
        Debug.Log($"[{nameof(PuzzleInteractable)}] {gameObject.name} unlocked and can now start the puzzle.", this);
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

        if (_haveToUnlock && !GetState(_unlockStateKey))
        {
            SetFailureResult(EUseInteractResult.Locked);
            failureReason = "The Puzzle Is not Unlocked";
            return false;
        }

        if (GetState(_completedStateKey))
        {
            SetFailureResult(EUseInteractResult.AlreadyCompleted);
            failureReason = "The puzzle is already completed.";
            return false;
        }

        if (PuzzleBase.IsSolved)
        {
            SetFailureResult(EUseInteractResult.AlreadyCompleted);
            failureReason = "The controller is already solved.";
            return false;
        }

        if (PuzzleBase.HasActivePuzzle)
        {
            SetFailureResult(EUseInteractResult.PuzzleAlreadyRunning);
            failureReason = "The puzzle is already running.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        PuzzleBase.TryOpen(this);
        Debug.Log($"[{nameof(PuzzleInteractable)}] {gameObject.name} started the puzzle.", this);
        _onPuzzleStarted?.Invoke();
        failureReason = string.Empty;
        SetFailureResult(EUseInteractResult.Success);
        return true;
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }

    protected override void OnUseSucceeded(InteractionContext context)
    {
        // Puzzle interactables manage their own completion lifecycle.
        // Skipping the base after-use handling prevents one-shot deactivation
        // from blocking re-entry after the player cancels the puzzle.
        PublishObjectInteracted(GetSuccessInteractEventType(context));
    }

    private bool ValidateConfiguration(out string failureReason)
    {
        if (PuzzleBase == null)
        {
            failureReason = "Puzzle Controller reference is missing.";
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
