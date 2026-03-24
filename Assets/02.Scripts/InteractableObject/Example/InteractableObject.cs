using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class InteractableObject : MonoBehaviour, IInteractObject
{
    [SerializeField] protected bool _isInteractActive;
    
    public event Action OnInteract;
    
    [Header("Scene Event")]
    public UnityEvent InteractEvent;
    public abstract void Interact();

    private void Start()
    {
        InteractController controller = GetComponentInParent<InteractController>();
        _isInteractActive = controller == null || controller.ScannableObject == null;
    }

    public void SetActivate()
    {
        _isInteractActive = true;
    }

    protected void OnInteractActivate()
    {
        OnInteract?.Invoke();
        InteractEvent?.Invoke();
    }
}
