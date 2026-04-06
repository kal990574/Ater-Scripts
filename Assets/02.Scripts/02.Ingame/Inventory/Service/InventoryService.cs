using System;
using System.Collections.Generic;

public class InventoryService
{
    private readonly List<string> _itemInstanceIds = new List<string>();
    private int _selectedIndex = -1;

    public IReadOnlyList<string> Items => _itemInstanceIds;
    public int Count => _itemInstanceIds.Count;
    public int SelectedIndex => _selectedIndex;

    public event Action OnInventoryChanged;
    public event Action<int> OnSelectionChanged;
    public event Action<string> OnItemAdded;
    public event Action<string> OnItemRemoved;

    public bool TryAdd(string instanceId)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            return false;
        }

        _itemInstanceIds.Add(instanceId);
        OnItemAdded?.Invoke(instanceId);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool Remove(string instanceId)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            return false;
        }

        int index = _itemInstanceIds.IndexOf(instanceId);
        if (index < 0)
        {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= _itemInstanceIds.Count)
        {
            return false;
        }

        string removedInstanceId = _itemInstanceIds[index];
        _itemInstanceIds.RemoveAt(index);

        if (_itemInstanceIds.Count == 0)
        {
            _selectedIndex = -1;
        }
        else if (_selectedIndex > index)
        {
            _selectedIndex--;
        }
        else if (_selectedIndex >= _itemInstanceIds.Count)
        {
            _selectedIndex = _itemInstanceIds.Count - 1;
        }

        OnItemRemoved?.Invoke(removedInstanceId);
        OnInventoryChanged?.Invoke();
        OnSelectionChanged?.Invoke(_selectedIndex);
        return true;
    }

    public bool Swap(int index1, int index2)
    {
        if (index1 < 0 || index1 >= _itemInstanceIds.Count || index2 < 0 || index2 >= _itemInstanceIds.Count)
        {
            return false;
        }

        string temp = _itemInstanceIds[index1];
        _itemInstanceIds[index1] = _itemInstanceIds[index2];
        _itemInstanceIds[index2] = temp;

        OnInventoryChanged?.Invoke();
        if (_selectedIndex == index1 || _selectedIndex == index2)
        {
            OnSelectionChanged?.Invoke(_selectedIndex);
        }

        return true;
    }

    public bool Select(int index)
    {
        if (index < 0 || index >= _itemInstanceIds.Count)
        {
            return false;
        }

        _selectedIndex = index;
        OnSelectionChanged?.Invoke(_selectedIndex);
        return true;
    }

    public void ClearSelection()
    {
        _selectedIndex = -1;
        OnSelectionChanged?.Invoke(_selectedIndex);
    }

    public int IndexOf(string instanceId)
    {
        return string.IsNullOrEmpty(instanceId)
            ? -1
            : _itemInstanceIds.IndexOf(instanceId);
    }

    public string GetAt(int index)
    {
        return index < 0 || index >= _itemInstanceIds.Count
            ? null
            : _itemInstanceIds[index];
    }
}
