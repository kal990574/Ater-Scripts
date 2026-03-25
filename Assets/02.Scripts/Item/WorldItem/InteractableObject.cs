using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class InteractableObject : MonoBehaviour, IInteractObject
{
    [Header("References")] 
    [SerializeField] protected int _initialItemKey;
    [SerializeField] protected bool _isInteractActive;
    
    private ItemInstance _instance;
    public ItemInstance ItemInstance => _instance;
    public event Action OnInteract;
    
    [Header("Scene Event")]
    public UnityEvent InteractEvent;
    public abstract void Interact();

    private void Start()
    {
        WorldItemController controller = GetComponentInParent<WorldItemController>();
        _isInteractActive = controller == null || controller.ScannableObject == null;
        
    }
    
    public void SetActivate()
    {
        _isInteractActive = true;
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
