using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WorldItemController : MonoBehaviour
{
    private IDetectableObject _detectableObject;
    private IScannableObject _scannableObject;
    private IInteractObject _interactableObject;
    private IItemBindable _itemBindableObject;
    private IInitialItemSource _initialItemSource;
    private IItemInstance _itemInstance;

    public IDetectableObject DetectableObject => _detectableObject;
    public IScannableObject ScannableObject => _scannableObject;
    public IInteractObject InteractableObject => _interactableObject;
    public IItemBindable ItemBindableObject => _itemBindableObject;

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

        if (_itemBindableObject == null)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] {gameObject.name} has no {nameof(IItemBindable)} to bind an ItemInstance.", this);
            return;
        }

        _itemBindableObject.SetInstance(instance);

        if (_itemInstance == null)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] {gameObject.name} has no {nameof(IItemInstance)}.", this);
            return;
        }

        _itemInstance.Bind(instance, InventoryManager.Instance);
        _itemInstance.RefreshView();
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
        if (_itemInstance == null)
        {
            _itemInstance = GetComponent<IItemInstance>();
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

        if (_itemBindableObject == null)
        {
            _itemBindableObject = GetComponentInChildren<IItemBindable>();
        }

        if (_initialItemSource == null)
        {
            _initialItemSource = GetComponentInChildren<IInitialItemSource>();
        }
    }

    private void EnsureGettableItemInstance()
    {
        CacheReferences();

        if (_itemBindableObject == null)
        {
            return;
        }

        if (_itemBindableObject.ItemInstance != null)
        {
            SetInstance(_itemBindableObject.ItemInstance);
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] {nameof(InventoryManager)}.Instance is null on {gameObject.name}.", this);
            return;
        }

        if (_initialItemSource == null)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] {gameObject.name} has no {nameof(IInitialItemSource)}.", this);
            return;
        }

        if (_initialItemSource.InitialItemKey < 0)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] {gameObject.name} has an invalid initial item key.", this);
            return;
        }

        ItemInstance instance = InventoryManager.Instance.CreateItemInstance(_initialItemSource.InitialItemKey);
        if (instance == null)
        {
            Debug.LogError($"[{nameof(WorldItemController)}] Failed to create ItemInstance for key {_initialItemSource.InitialItemKey} on {gameObject.name}.", this);
            return;
        }

        SetInstance(instance);
    }
}
