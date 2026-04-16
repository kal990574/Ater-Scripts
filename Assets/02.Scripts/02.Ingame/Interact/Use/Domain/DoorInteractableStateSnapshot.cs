public readonly struct DoorInteractableStateSnapshot
{
    public DoorInteractableStateSnapshot(bool isUnlocked, bool isOpen)
    {
        IsUnlocked = isUnlocked;
        IsOpen = isOpen;
    }

    public bool IsUnlocked { get; }
    public bool IsOpen { get; }
}
