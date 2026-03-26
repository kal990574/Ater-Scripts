using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public abstract class InteractableObject : MonoBehaviour, IInteractObject, IItemBindable
{
    [Header("References")] 
   
    [SerializeField] protected bool _isInteractActive;
    
    private IScannableObject _scannableObject;
    private IItemInstance _instance;
    public ItemInstance ItemInstance => _instance.ItemInstance;
    public event Action OnInteract;
    
    [Header("Scene Event")]
    public UnityEvent InteractEvent;

    private void Awake()
    {
        if (TryGetComponent(out IItemInstance instance))
        {
            _instance = instance;
        }
    }

    protected virtual void Start()
    {
        if (TryGetComponent(out IScannableObject scannableObject))
        {
            _scannableObject = scannableObject;
            _scannableObject.OnScanComplete += SetActivate;
        }
    }

    private void OnDestroy()
    {
        if (_scannableObject != null)
        {
            _scannableObject.OnScanComplete -= SetActivate;
        }
    }

    public abstract void Interact();
    
    public void SetActivate()
    {
        _isInteractActive = true;
        Debug.Log("활성화");
    }

    public void SetInstance(IItemInstance itemInstance)
    {
        _instance = itemInstance;
    }

    protected void OnInteractActivate()
    {
        OnInteract?.Invoke();
        InteractEvent?.Invoke();
    }
}
