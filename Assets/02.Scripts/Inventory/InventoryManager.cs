using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    public static InventoryManager Instance => _instance;

    [SerializeField] private ItemDataTable _table;

    //플레이어가 소유한 인벤토리
    private List<ItemData> _playerInventory = new List<ItemData>();
    
    [SerializeField]private bool _isInventoryUIOn = false;
    private int _selectedIndex = -1;
    
    public IReadOnlyList<ItemData> ReadonlyPlayerInventory => _playerInventory;
    public int Count => _playerInventory.Count;
    public int SelectedIndex => _selectedIndex;
    
    public event Action<bool> OnInventoryToggled;
    public event Action OnDataChanged;
    public event Action<int> OnSelectionChanged;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        
        // AddItem(_table.GetItem(1));
        // AddItem(_table.GetItem(1));
        // AddItem(_table.GetItem(1));
    }
    
    public void ClearSelection()
    {
        _selectedIndex = -1;
    }

    public void ToggleInventory()
    {
        _isInventoryUIOn = !_isInventoryUIOn;
        OnInventoryToggled?.Invoke(_isInventoryUIOn);
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= _playerInventory.Count) return;
        _selectedIndex = index;
        OnSelectionChanged?.Invoke(index);
    }

    public bool TryAddItem(int item)
    {
        //무조건 뒤에 넣는다
        ItemData newItem = _table.GetItem(item);
        if (newItem == null)
        {
            return false;
        }
        _playerInventory.Add(newItem);
        
        OnDataChanged?.Invoke();
        return true;
    }

    //인벤토리 인덱스로 삭제한다
    public void RemoveItem(int index)
    {
        _playerInventory.RemoveAt(index);

        if (_playerInventory.Count == 0)
        {
            _selectedIndex = -1;
        }

        else if (_selectedIndex > index)
        {
            _selectedIndex--;
        }
        else if (_selectedIndex >= _playerInventory.Count)
        {
            _selectedIndex = _playerInventory.Count - 1;
        }

        OnDataChanged?.Invoke();
        OnSelectionChanged?.Invoke(_selectedIndex);
    }

    public bool HasItem(int itemId)
    {
        foreach (ItemData item in _playerInventory)
        {
            if (item.ItemId == itemId) return true;
        }

        return false;
    }

    public void SwapItem(int index1, int index2)
    {
        var temp = _playerInventory[index1];
        _playerInventory[index1] = _playerInventory[index2];
        _playerInventory[index2] = temp;

        OnDataChanged?.Invoke();

        if (_selectedIndex == -1)
        {
            SelectItem(index2);
        }
        else if (_selectedIndex == index1 || _selectedIndex == index2)
        {
            SelectItem(_selectedIndex);
        }
    }
}