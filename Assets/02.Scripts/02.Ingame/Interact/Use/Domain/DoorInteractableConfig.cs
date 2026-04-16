public readonly struct DoorInteractableConfig
{
    public DoorInteractableConfig(string unlockStateKey, string openStateKey, string openAnimationStateName)
    {
        UnlockStateKey = unlockStateKey;
        OpenStateKey = openStateKey;
        OpenAnimationStateName = openAnimationStateName;
    }

    public string UnlockStateKey { get; }
    public string OpenStateKey { get; }
    public string OpenAnimationStateName { get; }
}
