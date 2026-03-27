using System;

public interface IItemBindable
{
    int InitialItemKey { get; }
    ItemInstanceData ItemInstanceData { get; }
    void SetInstance(IItemInstance itemInstance);
}


public interface IInteractObject
{
    ItemInstanceData ItemInstanceData { get; }
    event Action OnInteract;
    void Interact(UseContext context);
    void SetActivate(bool active);
}
