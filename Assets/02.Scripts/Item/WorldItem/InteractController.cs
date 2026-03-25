using System;
using UnityEngine;

[DisallowMultipleComponent]
public class InteractController : MonoBehaviour
{
    private IDetectableObject _detectableObject;
    private IScannableObject _scannableObject;
    private IInteractObject _interactableObject;

    public IDetectableObject DetectableObject => _detectableObject;
    public IScannableObject ScannableObject => _scannableObject;
    public IInteractObject InteractableObject => _interactableObject;


    private void Awake()
    {
        CacheReferences();
        BindScannableToInteractable();
    }

    private void OnDestroy()
    {
        UnbindScannableToInteractable();
    }

    private void OnValidate()
    {
        CacheReferences();
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

    private void BindScannableToInteractable()
    {
        if (_scannableObject == null || _interactableObject == null)
        {
            return;
        }
        
        _scannableObject.OnScanComplete += _interactableObject.SetActivate;
    }

    private void UnbindScannableToInteractable()
    {
        if (_scannableObject == null || _interactableObject == null)
        {
            return;
        }

        _scannableObject.OnScanComplete -= _interactableObject.SetActivate;
    }
}
