public static class DoorInteractableValidator
{
    public static bool Validate(DoorInteractableConfig config, bool hasRuntimeState, out string failureReason)
    {
        if (string.IsNullOrWhiteSpace(config.UnlockStateKey))
        {
            failureReason = "Unlock state key is not configured.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(config.OpenStateKey))
        {
            failureReason = "Open state key is not configured.";
            return false;
        }

        if (!hasRuntimeState)
        {
            failureReason = "RuntimeData.State is not available.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }
}
