using UnityEngine;

public class InteractController : MonoBehaviour
{
    [Header("Force Switches")]
    [SerializeField] private bool _forceDetectEnableSwitch;
    [SerializeField] private bool _forceScanEnableSwitch;
    [SerializeField] private bool _forceInteractEnableSwitch;
    
    private IDetectableObject _detectableObject;
    private IScannableObject _scannableObject;
    private IInteractObject _interactableObject;

    public IDetectableObject DetectableObject => _detectableObject;
    public IScannableObject ScannableObject => _scannableObject;
    public IInteractObject InteractableObject => _interactableObject;

    private void Awake()
    {
        if (_detectableObject == null)
        {
            _detectableObject = GetComponentInChildren<IDetectableObject>();
            _forceDetectEnableSwitch = false;
            _forceScanEnableSwitch = false;
            _forceInteractEnableSwitch = false;
        }

        if (_scannableObject == null)
        {
            _scannableObject = GetComponentInChildren<IScannableObject>();
            _forceScanEnableSwitch = false;
            _forceInteractEnableSwitch = false;
        }
        
        if (_interactableObject == null)
        {
            _interactableObject = GetComponentInChildren<IInteractObject>();
            _forceInteractEnableSwitch = false;
        }
    }

    
    public void OnDetectEnter()
    {
        if (!_forceDetectEnableSwitch)
        {
            return;
        }
        
        if (_detectableObject == null)
        {
            return;
        }
        
        _detectableObject?.OnDetectEnter();
    }

    public void OnDetectExit()
    {
        if (!_forceDetectEnableSwitch)
        {
            return;
        }

        if (_detectableObject == null)
        {
            return;
        }
        
        _detectableObject?.OnDetectExit();
    }

    public bool TryInteract()
    {
        if (!_forceInteractEnableSwitch)
        {
            return false;
        }
        
        if (_interactableObject == null)
        {
            return false;
        }

        _interactableObject.Interact();
        return true;
    }
}
