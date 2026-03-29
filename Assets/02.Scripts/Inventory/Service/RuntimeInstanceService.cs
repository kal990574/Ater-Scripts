using System.Collections.Generic;
using UnityEngine;

public class RuntimeInstanceService
{
    private ItemDataTableSO itemDataTableSo;
    
    //생성된 아이템의 인스턴스를 보관
    private readonly Dictionary<string, RuntimeItemData> _instances = new();

    public RuntimeInstanceService(ItemDataTableSO itemDataTableSo)
    {
        this.itemDataTableSo = itemDataTableSo;
    }

    public void SetItemDataTable(ItemDataTableSO itemDataTableSo)
    {
        this.itemDataTableSo = itemDataTableSo;
    }

    public RuntimeItemData CreateInstance(int itemId)
    {
        if (itemDataTableSo == null)
        {
            Debug.LogError($"[{nameof(RuntimeInstanceService)}] {nameof(ItemDataTableSO)} reference is missing.");
            return null;
        }

        RuntimeItemData runtimeItem = new RuntimeItemData(itemDataTableSo.GetItemData(itemId));
        if (runtimeItem == null)
        {
            return null;
        }

        RegisterInstance(runtimeItem);
        return runtimeItem;
    }

    public bool RegisterInstance(RuntimeItemData runtimeItem)
    {
        if (runtimeItem == null || string.IsNullOrEmpty(runtimeItem.InstanceId))
        {
            return false;
        }

        _instances[runtimeItem.InstanceId] = runtimeItem;
        return true;
    }

    public bool TryGetInstance(string instanceId, out RuntimeItemData runtimeItem)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            runtimeItem = null;
            return false;
        }

        return _instances.TryGetValue(instanceId, out runtimeItem);
    }

    public RuntimeItemData GetInstance(string instanceId)
    {
        TryGetInstance(instanceId, out RuntimeItemData itemInstance);
        return itemInstance;
    }
}
