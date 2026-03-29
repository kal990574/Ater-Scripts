using System;

public interface IInteractObject
{
    RuntimeItemData RuntimeItemData { get; }
    event Action OnInteract;
    void Interact(UseContext context);
    void SetActivate(bool active);
}
