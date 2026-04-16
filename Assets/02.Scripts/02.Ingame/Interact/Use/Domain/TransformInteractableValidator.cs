public static class TransformInteractableValidator
{
    public static bool Validate(TransformInteractableSettings settings, out string failureReason)
    {
        if (settings.TransformToMove == null)
        {
            failureReason = "Transform to move is not assigned.";
            return false;
        }

        if (settings.MoveDuration < 0f)
        {
            failureReason = "Move duration must be zero or greater.";
            return false;
        }

        if (settings.RotateDuration < 0f)
        {
            failureReason = "Rotate duration must be zero or greater.";
            return false;
        }

        if (settings.DestinationMode == TransformInteractable.EDestinationMode.TargetTransform && settings.DestinationTransform == null)
        {
            failureReason = "Destination transform is not assigned.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }
}
