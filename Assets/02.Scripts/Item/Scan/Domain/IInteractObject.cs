using System;

public interface IItemBindable
{
    ItemInstance ItemInstance { get; }
    void SetInstance(ItemInstance itemInstance);
}

public interface IInitialItemSource
{
    int InitialItemKey { get; }
}

public interface IInteractObject
{
    ItemInstance ItemInstance { get; }
    event Action OnInteract;
    void Interact();
    void SetActivate();
}
