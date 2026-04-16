public static class TransformInteractablePoseResolver
{
    public static TransformInteractablePose Resolve(TransformInteractableSettings settings)
    {
        switch (settings.DestinationMode)
        {
            case TransformInteractable.EDestinationMode.TargetTransform:
                return new TransformInteractablePose(
                    settings.DestinationTransform.position,
                    settings.DestinationTransform.rotation.eulerAngles,
                    false);

            case TransformInteractable.EDestinationMode.LocalPose:
                return new TransformInteractablePose(
                    settings.LocalPosition,
                    settings.LocalEulerAngles,
                    true);

            default:
                return new TransformInteractablePose(
                    settings.WorldPosition,
                    settings.WorldEulerAngles,
                    false);
        }
    }
}
