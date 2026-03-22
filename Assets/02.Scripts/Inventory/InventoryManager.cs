using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private ItemDataTable _table;

    private List<ItemData> _playerInventory = new List<ItemData>();
    public IReadOnlyList<ItemData> ReadonlyPlayerInventory => _playerInventory;

    // todo : 인벤토리 껐다가 키는 기능
    private bool IsInventoryUIOn = false;

    public int Count => _playerInventory.Count;

    public event Action<bool> OnInventoryToggled;

    public event Action OnDataChanged;




    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        AddItem(_table.GetItem(1));
        AddItem(_table.GetItem(1));
        AddItem(_table.GetItem(1));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleInventory();
    }

    public void ToggleInventory()
    {
        IsInventoryUIOn = !IsInventoryUIOn;
        OnInventoryToggled?.Invoke(IsInventoryUIOn);
    }

    public bool AddItem(ItemData item)
    {
        //무조건 뒤에 넣는다
        if (item == null) return false;

        _playerInventory.Add(item);
        OnDataChanged?.Invoke();

        return true;
    }

    //인벤토리 인덱스로 삭제한다
    public bool RemoveItem(int number)
    {
        if (number < 0 || number >= _playerInventory.Count)
            return false;

        _playerInventory.RemoveAt(number);
        OnDataChanged?.Invoke();
        return true;
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
    }
}