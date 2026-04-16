public static class DoorInteractableStatePolicy
{
    public static UseInteractionOutcome EvaluateOpen(DoorInteractableStateSnapshot state)
    {
        if (state.IsOpen)
        {
            return UseInteractionOutcome.Fail(EUseInteractResult.AlreadyOpen, "The door is already open.");
        }

        if (!state.IsUnlocked)
        {
            return UseInteractionOutcome.Fail(EUseInteractResult.Locked, "The door is locked.");
        }

        return UseInteractionOutcome.Success();
    }

    public static bool CanUnlock(DoorInteractableStateSnapshot state, out string failureReason)
    {
        if (state.IsUnlocked)
        {
            failureReason = "The door is already unlocked.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }
}
