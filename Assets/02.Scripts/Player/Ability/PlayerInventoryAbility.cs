using _02.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryAbility : PlayerAbility
{
    [SerializeField] private Transform _handRoot;
    [SerializeField] private float _throwDistance = 1.5f;
    
    
    [SerializeField]private int _handIndex = -1;
    
    private readonly Dictionary<string, GameObject> _handItemCache = new();
    
    private InventoryManager _inventoryManager;
    private ItemInstance _currentHandItem;
    private GameObject _currentHandObject;
    
    private void Start()
    {
        _inventoryManager = InventoryManager.Instance;
    }
    
    public void ToggleInventory()
    {
        if (_inventoryManager == null)
        {
            return;
        }
        
        _inventoryManager.ToggleInventory();
    }

    //0~4
    public bool TryPickUpItem(int num)
    {
        if (_inventoryManager == null || _handRoot == null)
        {
            return false;
        }

        if (num < 0 || num >= _inventoryManager.ReadonlyPlayerInventory.Count)
        {
            return false;
        }

        
        ItemInstance itemInstance = _inventoryManager.ReadonlyPlayerInventory[num];
        if (itemInstance == null)
        {
            return false;
        }
        
        ClearHandItem();
        _handIndex = num;
        _currentHandItem = itemInstance;

        if (_handItemCache.TryGetValue(itemInstance.InstanceId, out GameObject cachedHandItem) && cachedHandItem != null)
        {
            _currentHandObject = cachedHandItem;
            _currentHandObject.transform.SetParent(_handRoot, false);
            _currentHandObject.transform.localPosition = Vector3.zero;
            _currentHandObject.transform.localRotation = Quaternion.identity;
            _currentHandObject.transform.localScale = Vector3.one;
            _currentHandObject.SetActive(true);
            return true;
        }

        _currentHandObject = _inventoryManager.CreateHandItem(itemInstance, _handRoot);
        if (_currentHandObject == null)
        {
            _currentHandItem = null;
            return false;
        }

        _handItemCache[itemInstance.InstanceId] = _currentHandObject;
        return true;
    }
    
    public bool TryThrowItem()
    {
        if (_inventoryManager == null || _currentHandItem == null)
        {
            return false;
        }
        
        GameObject worldObject = _inventoryManager.CreateWorldItem(_currentHandItem, null);
        if (worldObject == null)
        {
            return false;
        }

        worldObject.transform.position = _owner.transform.position + (_owner.transform.forward * _throwDistance);
        worldObject.transform.rotation = Quaternion.identity;

        RemoveCurrentHandItemFromCache();
        _inventoryManager.RemoveItem(_handIndex);

        int nextHandIndex = GetNextHandIndexAfterThrow();
        ClearHandItem();
        TryPickUpItem(nextHandIndex);
        return true;
    }
    

    public void ClearHandItem()
    {
        _handIndex = -1;
        _currentHandItem = null;

        if (_currentHandObject == null)
        {
            return;
        }

        if (IsCachedHandItem(_currentHandObject))
        {
            _currentHandObject.SetActive(false);
            _currentHandObject.transform.SetParent(null);
        }
        else
        {
            Destroy(_currentHandObject);
        }

        _currentHandObject = null;
    }

    private bool IsCachedHandItem(GameObject handObject)
    {
        foreach (KeyValuePair<string, GameObject> pair in _handItemCache)
        {
            if (pair.Value == handObject)
            {
                return true;
            }
        }

        return false;
    }

    private void RemoveCurrentHandItemFromCache()
    {
        if (_currentHandItem == null)
        {
            return;
        }

        if (_handItemCache.Remove(_currentHandItem.InstanceId, out GameObject cachedHandObject) && cachedHandObject != null)
        {
            Destroy(cachedHandObject);
        }
    }

    private int GetNextHandIndexAfterThrow()
    {
        int itemCount = _inventoryManager.ReadonlyPlayerInventory.Count;
        if (itemCount <= 0)
        {
            return -1;
        }

        if (_handIndex < itemCount)
        {
            return _handIndex;
        }

        return itemCount - 1;
    }
}
