using System.Collections.Generic;
using UnityEngine;

public class RuntimeInstanceService
{
    private ItemDataTable _itemDataTable;
    private readonly Dictionary<string, InstanceData> _instances = new();

    public RuntimeInstanceService(ItemDataTable itemDataTable)
    {
        _itemDataTable = itemDataTable;
    }

    public void SetItemDataTable(ItemDataTable itemDataTable)
    {
        _itemDataTable = itemDataTable;
    }

    public InstanceData CreateInstance(int itemId)
    {
        if (_itemDataTable == null)
        {
            Debug.LogError($"[{nameof(RuntimeInstanceService)}] {nameof(ItemDataTable)} reference is missing.");
            return null;
        }

        InstanceData instance = new InstanceData(_itemDataTable.GetItemData(itemId));
        if (instance == null)
        {
            return null;
        }

        RegisterInstance(instance);
        return instance;
    }

    public bool RegisterInstance(InstanceData instance)
    {
        if (instance == null || string.IsNullOrEmpty(instance.InstanceId))
        {
            return false;
        }

        _instances[instance.InstanceId] = instance;
        return true;
    }

    public bool TryGetInstance(string instanceId, out InstanceData instance)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            instance = null;
            return false;
        }

        return _instances.TryGetValue(instanceId, out instance);
    }

    public InstanceData GetInstance(string instanceId)
    {
        TryGetInstance(instanceId, out InstanceData itemInstance);
        return itemInstance;
    }
}
