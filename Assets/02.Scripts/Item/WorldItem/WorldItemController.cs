using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WorldItemController : MonoBehaviour
{
    private IDetectableObject _detectableObject;
    private IScannableObject _scannableObject;
    private IInteractObject _interactableObject;
    private GettableObject _gettableObject;
    private WorldItemBinder _worldItemBinder;

    public IDetectableObject DetectableObject => _detectableObject;
    public IScannableObject ScannableObject => _scannableObject;
    public IInteractObject InteractableObject => _interactableObject;
    public GettableObject GettableObject => _gettableObject;

    private void Awake()
    {
        CacheReferences();
        if (_scannableObject == null || _interactableObject == null)
        {
            return;
        }

        _scannableObject.OnScanComplete += _interactableObject.SetActivate;
    }

    private void Start()
    {
        EnsureGettableItemInstance();
    }

    public void SetInstance(ItemInstance instance)
    {
        if (instance == null)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] Tried to bind a null ItemInstance on {gameObject.name}.", this);
            return;
        }

        CacheReferences();

        if (_gettableObject == null)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] {gameObject.name} has no {nameof(GettableObject)} to bind an ItemInstance.", this);
            return;
        }

        _gettableObject.SetInstance(instance);

        if (_worldItemBinder == null)
        {
            _worldItemBinder = GetComponent<WorldItemBinder>();
        }

        if (_worldItemBinder == null)
        {
            _worldItemBinder = gameObject.AddComponent<WorldItemBinder>();
        }

        _worldItemBinder.Bind(instance, InventoryManager.Instance);
        _worldItemBinder.RefreshView();
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
        if (_worldItemBinder == null)
        {
            _worldItemBinder = GetComponent<WorldItemBinder>();
        }

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

        if (_gettableObject == null)
        {
            _gettableObject = GetComponentInChildren<GettableObject>();
        }
    }

    private void EnsureGettableItemInstance()
    {
        CacheReferences();

        if (_gettableObject == null)
        {
            return;
        }

        if (_gettableObject.ItemInstance != null)
        {
            SetInstance(_gettableObject.ItemInstance);
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] {nameof(InventoryManager)}.Instance is null on {gameObject.name}.", this);
            return;
        }

        if (_gettableObject.InitialItemKey < 0)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] {gameObject.name} has an invalid initial item key.", this);
            return;
        }

        ItemInstance instance = InventoryManager.Instance.CreateItemInstance(_gettableObject.InitialItemKey);
        if (instance == null)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] Failed to create ItemInstance for key {_gettableObject.InitialItemKey} on {gameObject.name}.", this);
            return;
        }

        SetInstance(instance);
    }
}
