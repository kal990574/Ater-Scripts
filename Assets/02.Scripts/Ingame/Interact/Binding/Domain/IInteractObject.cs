using System;

public interface IItemBindable
{
    int InitialItemKey { get; }
    InstanceData InstanceData { get; }
    void SetInstance(IItemInstance itemInstance);
}


public interface IInteractObject
{
    InstanceData InstanceData { get; }
    event Action OnInteract;
    void Interact(UseContext context);
    void SetActivate(bool active);
}
