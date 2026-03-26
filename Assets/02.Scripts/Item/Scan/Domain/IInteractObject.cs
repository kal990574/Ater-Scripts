using System;

public interface IItemBindable
{
    int InitialItemKey { get; }
    ItemInstance ItemInstance { get; }
    void SetInstance(IItemInstance itemInstance);
}


public interface IInteractObject
{
    ItemInstance ItemInstance { get; }
    event Action OnInteract;
    void Interact(UseContext context);
    void SetActivate(bool active);
}
