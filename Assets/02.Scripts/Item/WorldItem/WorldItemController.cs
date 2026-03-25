using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WorldItemController : MonoBehaviour
{
    //캐싱
    private IDetectableObject _detectableObject;
    private IScannableObject _scannableObject;
    private IInteractObject _interactableObject;
    
    public IDetectableObject DetectableObject => _detectableObject;
    public IScannableObject ScannableObject => _scannableObject;
    public IInteractObject InteractableObject => _interactableObject;


    private void Awake()
    {
        CacheReferences();
        if (_scannableObject == null || _interactableObject == null)
        {
            return;
        }
        _scannableObject.OnScanComplete += _interactableObject.SetActivate;
    }

    public void SetInstance(ItemInstance instance)
    {
        
    }

    private void OnDestroy()
    {
        if (_scannableObject == null || _interactableObject == null)
        {
            return;
        }

        _scannableObject.OnScanComplete -= _interactableObject.SetActivate;
    }
    

    public void OnDetectEnter()
    {
        _detectableObject?.OnDetectEnter();
    }

    public void OnDetectExit()
    {
        _detectableObject?.OnDetectExit();
    }

    public bool TryInteract()
    {
        if (_interactableObject == null)
        {
            return false;
        }

        _interactableObject.Interact();
        return true;
    }

    private void CacheReferences()
    {
        if (_detectableObject == null)
        {
            _detectableObject = GetComponentInChildren<IDetectableObject>();
        }

        if (_scannableObject == null)
        {
            _scannableObject = GetComponentInChildren<IScannableObject>();
        }

        if (_interactableObject == null)
        {
            _interactableObject = GetComponentInChildren<IInteractObject>();
        }
    }
}
