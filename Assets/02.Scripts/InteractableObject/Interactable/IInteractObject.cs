using System;

public interface IInteractObject
{
    event Action OnInteract;
    void Interact();
    void SetActivate();
}