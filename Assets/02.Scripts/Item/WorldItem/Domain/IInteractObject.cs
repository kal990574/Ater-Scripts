using System;

public interface IInteractObject
{
    ItemInstance ItemInstance { get; }
    event Action OnInteract;
    void Interact();
    void SetActivate();
    void SetInstance(ItemInstance itemInstance);
}