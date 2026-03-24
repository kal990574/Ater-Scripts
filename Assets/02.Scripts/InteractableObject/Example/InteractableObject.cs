using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class InteractableObject : MonoBehaviour, IInteractObject
{
    public event Action OnInteract;
    
    [Header("Scene Event")]
    public UnityEvent InteractEvent;
    public abstract void Interact();
    

    protected void OnInteractActivate()
    {
        OnInteract?.Invoke();
        InteractEvent?.Invoke();
    }
}
