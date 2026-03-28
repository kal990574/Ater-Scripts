using System.Collections.Generic;
using UnityEngine;

public class RuntimeInstanceService
{
    private ItemDataTable _itemDataTable;
    private readonly Dictionary<string, ItemInstanceData> _instances = new();

    public RuntimeInstanceService(ItemDataTable itemDataTable)
    {
        _itemDataTable = itemDataTable;
    }

    public void SetItemDataTable(ItemDataTable itemDataTable)
    {
        _itemDataTable = itemDataTable;
    }

    public ItemInstanceData CreateInstance(int itemId)
    {
        return CreateInstance(itemId, null, null);
    }

    public ItemInstanceData CreateInstance(int itemId, string instanceId, ItemState state)
    {
        if (_itemDataTable == null)
        {
            Debug.LogError($"[{nameof(RuntimeInstanceService)}] {nameof(ItemDataTable)} reference is missing.");
            return null;
        }

        ItemData itemData = _itemDataTable.GetItemData(itemId);
        if (itemData == null)
        {
            return null;
        }

        ItemInstanceData itemInstance = new ItemInstanceData(instanceId, itemData, state);
        if (itemInstance == null)
        {
            return null;
        }

        RegisterInstance(itemInstance);
        return itemInstance;
    }

    public bool RegisterInstance(ItemInstanceData itemInstance)
    {
        if (itemInstance == null || string.IsNullOrEmpty(itemInstance.InstanceId))
        {
            return false;
        }

        _instances[itemInstance.InstanceId] = itemInstance;
        return true;
    }

    public bool TryGetInstance(string instanceId, out ItemInstanceData itemInstance)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            itemInstance = null;
            return false;
        }

        return _instances.TryGetValue(instanceId, out itemInstance);
    }

    public ItemInstanceData GetInstance(string instanceId)
    {
        TryGetInstance(instanceId, out ItemInstanceData itemInstance);
        return itemInstance;
    }

    public IReadOnlyCollection<ItemInstanceData> GetAllInstances()
    {
        return _instances.Values;
    }

    public void Clear()
    {
        _instances.Clear();
    }

    public ItemInstanceData RestoreInstance(ItemInstanceSaveData saveData)
    {
        if (saveData == null)
        {
            return null;
        }

        ItemState state = new ItemState();
        state.RestoreSaveData(saveData.StateEntries);
        return CreateInstance(saveData.ItemId, saveData.InstanceId, state);
    }
}
