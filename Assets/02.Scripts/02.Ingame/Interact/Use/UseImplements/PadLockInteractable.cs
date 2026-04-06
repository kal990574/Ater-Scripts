using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PadLockInteractable : UsableObject
{
    [SerializeField] private PadLockController _padLockController;

    protected override void OnAwake()
    {
        base.OnAwake();
        GetComponentCached(ref _padLockController);
    }

    protected override bool CanUse(InteractionContext context, out string failureReason)
    {
        if (_padLockController == null)
        {
            SetFailureResult(UseInteractResult.InvalidConfiguration);
            failureReason = "PadLockController reference is missing.";
            return false;
        }

        if (_padLockController.IsSolved)
        {
            SetFailureResult(UseInteractResult.AlreadyUnlocked);
            failureReason = "The padlock puzzle is already solved.";
            return false;
        }

        if (_padLockController.HasActivePuzzle)
        {
            SetFailureResult(UseInteractResult.PuzzleAlreadyRunning);
            failureReason = "The padlock puzzle is already running.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }

    protected override bool OnUse(InteractionContext context, out string failureReason)
    {
        _padLockController.TryOpen();
        Debug.Log($"[{nameof(PadLockInteractable)}] {gameObject.name} started the padlock puzzle.", this);
        failureReason = string.Empty;
        SetFailureResult(UseInteractResult.Success);
        return true;
    }

    protected override void OnUseFailed(InteractionContext context, string failureReason)
    {
        _onInteractionFailed?.Invoke();
    }
}
