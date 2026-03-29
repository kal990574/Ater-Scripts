using System;

public interface IInteractObject
{
    RuntimeData RuntimeData { get; }
    RuntimeItemData RuntimeItemData { get; }
    event Action OnInteract;
    void Interact(UseContext context);
    void SetActivate(bool active);
}
