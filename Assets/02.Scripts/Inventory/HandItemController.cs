using UnityEngine;

public class HandItemController : MonoBehaviour
{
    [SerializeField] private Transform _handRoot;

    private InventoryItemInstance _currentItemInstance;
    private GameObject _currentHandObject;

    public InventoryItemInstance CurrentItemInstance => _currentItemInstance;
    public GameObject CurrentHandObject => _currentHandObject;

    public void ShowItem(InventoryItemInstance itemInstance)
    {
        ClearItem();

        if (_handRoot == null || itemInstance == null)
        {
            return;
        }

        GameObject prefab = itemInstance.Definition != null ? itemInstance.Definition.HandPrefab : itemInstance.Prefab;
        if (prefab == null)
        {
            return;
        }

        _currentItemInstance = itemInstance;
        _currentHandObject = Instantiate(prefab, _handRoot, false);
        _currentHandObject.transform.localPosition = Vector3.zero;
        _currentHandObject.transform.localRotation = Quaternion.identity;
        _currentHandObject.transform.localScale = Vector3.one;

        HandItemBinder binder = _currentHandObject.GetComponent<HandItemBinder>();
        if (binder == null)
        {
            binder = _currentHandObject.AddComponent<HandItemBinder>();
        }

        binder.Bind(itemInstance, InventoryManager.Instance);
    }

    public void RefreshCurrentItem()
    {
        if (_currentHandObject == null || _currentItemInstance == null)
        {
            return;
        }

        HandItemBinder binder = _currentHandObject.GetComponent<HandItemBinder>();
        if (binder == null)
        {
            return;
        }

        binder.Bind(_currentItemInstance, InventoryManager.Instance);
    }

    public void ClearItem()
    {
        _currentItemInstance = null;

        if (_currentHandObject == null)
        {
            return;
        }

        Destroy(_currentHandObject);
        _currentHandObject = null;
    }
}
