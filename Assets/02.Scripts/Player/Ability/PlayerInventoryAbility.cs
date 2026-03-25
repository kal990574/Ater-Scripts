using _02.Scripts.Player;
using UnityEngine;

public class PlayerInventoryAbility : PlayerAbility
{
    
    [SerializeField] private float _throwDistance = 1.5f;
    [SerializeField] private int _handIndex = -1;
    
    private InventoryManager _inventoryManager;
    private ItemInstance _currentHandItem;
    
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

    public bool TryPickUpItem(int num)
    {
        if (_inventoryManager == null)
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
        
        _inventoryManager.HideHandItem();
        _handIndex = num;
        _currentHandItem = itemInstance;

        return _inventoryManager.ShowHandItem(itemInstance) != null;
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

        int nextHandIndex = GetNextHandIndexAfterThrow();
        _inventoryManager.RemoveItem(_handIndex);
        ClearHandItem();
        TryPickUpItem(nextHandIndex);
        return true;
    }
    
    public void ClearHandItem()
    {
        _handIndex = -1;
        _currentHandItem = null;
        _inventoryManager?.HideHandItem();
    }

    private int GetNextHandIndexAfterThrow()
    {
        int itemCountAfterRemoval = _inventoryManager.ReadonlyPlayerInventory.Count - 1;
        if (itemCountAfterRemoval <= 0)
        {
            return -1;
        }

        if (_handIndex < itemCountAfterRemoval)
        {
            return _handIndex;
        }

        return itemCountAfterRemoval - 1;
    }
}
