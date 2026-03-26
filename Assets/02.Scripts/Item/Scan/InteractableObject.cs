using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class InteractableObject : MonoBehaviour, IInteractObject, IItemBindable
{
    [Header("References")] 
   
    [SerializeField] protected bool _isInteractActive;
    
    private ItemInstance _instance;
    
    public ItemInstance ItemInstance => _instance;
    public event Action OnInteract;
    
    [Header("Scene Event")]
    public UnityEvent InteractEvent;
    public abstract void Interact();
    
    public void SetActivate()
    {
        _isInteractActive = true;
        Debug.Log("활성화");
    }

    public void SetInstance(ItemInstance itemInstance)
    {
        _instance = itemInstance;
    }

    protected void OnInteractActivate()
    {
        OnInteract?.Invoke();
        InteractEvent?.Invoke();
    }
}
