using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PadLockInteractable : UsableObject
{
    [SerializeField] private PadLockController padLockController;

    protected override void OnAwake()
    {
        base.OnAwake();
        GetComponentCached(ref padLockController);
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (padLockController == null)
        {
            SetFailureResult(EUseInteractResult.InvalidConfiguration);
            failureReason = "PadLockController reference is missing.";
            return false;
        }

        if (padLockController.IsSolved)
        {
            SetFailureResult(EUseInteractResult.AlreadyUnlocked);
            failureReason = "The padlock puzzle is already solved.";
            return false;
        }

        if (padLockController.HasActivePuzzle)
        {
            SetFailureResult(EUseInteractResult.PuzzleAlreadyRunning);
            failureReason = "The padlock puzzle is already running.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        padLockController.TryOpen(null);
        Debug.Log($"[{nameof(PadLockInteractable)}] {gameObject.name} started the padlock puzzle.", this);
        failureReason = string.Empty;
        SetFailureResult(EUseInteractResult.Success);
        return true;
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }
}
