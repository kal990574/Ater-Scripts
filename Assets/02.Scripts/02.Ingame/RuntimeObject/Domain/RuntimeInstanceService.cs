using System.Collections.Generic;
using UnityEngine;

public class RuntimeInstanceService
{
    private ItemDataTableSO itemDataTableSo;
    private readonly Dictionary<string, RuntimeData> _instances = new();

    public IReadOnlyDictionary<string, RuntimeData> Instances => _instances;

    public RuntimeInstanceService(ItemDataTableSO itemDataTableSo)
    {
        this.itemDataTableSo = itemDataTableSo;
    }

    public void SetItemDataTable(ItemDataTableSO itemDataTableSo)
    {
        this.itemDataTableSo = itemDataTableSo;
    }

    public RuntimeData CreateRuntimeData(string objectName = null, InteractState defaultState = null)
    {
        RuntimeData runtimeData = new RuntimeData(null, objectName, defaultState);
        RegisterInstance(runtimeData);
        return runtimeData;
    }

    public RuntimeData CreateRuntimeData(InteractState defaultState)
    {
        return CreateRuntimeData(null, defaultState);
    }

    public RuntimeData GetOrCreateRuntimeData(string instanceId, string objectName = null, InteractState defaultState = null)
    {
        if (TryGetRuntimeData(instanceId, out RuntimeData runtimeData))
        {
            return runtimeData;
        }

        runtimeData = new RuntimeData(instanceId, objectName, defaultState);
        RegisterInstance(runtimeData);
        return runtimeData;
    }

    public RuntimeData GetOrCreateRuntimeData(string instanceId, InteractState defaultState)
    {
        return GetOrCreateRuntimeData(instanceId, null, defaultState);
    }

    public RuntimeItemData CreateInstance(int itemId, string objectName = null)
    {
        if (itemDataTableSo == null)
        {
            Debug.LogError($"[{nameof(RuntimeInstanceService)}] {nameof(ItemDataTableSO)} reference is missing.");
            return null;
        }

        RuntimeItemData runtimeItem = new RuntimeItemData(null, objectName, itemDataTableSo.GetItemData(itemId));
        if (runtimeItem == null)
        {
            return null;
        }

        RegisterInstance(runtimeItem);
        return runtimeItem;
    }

    public RuntimeItemData GetOrCreateItemInstance(string instanceId, int itemId, string objectName = null)
    {
        if (TryGetRuntimeData(instanceId, out RuntimeData runtimeData))
        {
            if (runtimeData is RuntimeItemData runtimeItemData)
            {
                return runtimeItemData;
            }

            Debug.LogError($"[{nameof(RuntimeInstanceService)}] Instance '{instanceId}' already exists as {nameof(RuntimeData)}.");
            return null;
        }

        if (itemDataTableSo == null)
        {
            Debug.LogError($"[{nameof(RuntimeInstanceService)}] {nameof(ItemDataTableSO)} reference is missing.");
            return null;
        }

        ItemData itemData = itemDataTableSo.GetItemData(itemId);
        if (itemData == null)
        {
            return null;
        }

        RuntimeItemData runtimeItem = new RuntimeItemData(instanceId, objectName, itemData);
        RegisterInstance(runtimeItem);
        return runtimeItem;
    }

    public RuntimeItemData CreateInstance(int itemId)
    {
        return CreateInstance(itemId, null);
    }

    public RuntimeItemData GetOrCreateItemInstance(string instanceId, int itemId)
    {
        return GetOrCreateItemInstance(instanceId, itemId, null);
    }

    public bool RegisterInstance(RuntimeData runtimeData)
    {
        if (runtimeData == null || string.IsNullOrEmpty(runtimeData.InstanceId))
        {
            return false;
        }

        _instances[runtimeData.InstanceId] = runtimeData;
        return true;
    }

    public bool TryGetRuntimeData(string instanceId, out RuntimeData runtimeData)
    {
        if (string.IsNullOrEmpty(instanceId))
        {
            runtimeData = null;
            return false;
        }

        return _instances.TryGetValue(instanceId, out runtimeData);
    }

    public RuntimeData GetRuntimeData(string instanceId)
    {
        TryGetRuntimeData(instanceId, out RuntimeData runtimeData);
        return runtimeData;
    }

    public bool TryGetItemInstance(string instanceId, out RuntimeItemData runtimeItem)
    {
        runtimeItem = GetRuntimeData(instanceId) as RuntimeItemData;
        return runtimeItem != null;
    }

    public RuntimeItemData GetItemInstance(string instanceId)
    {
        return GetRuntimeData(instanceId) as RuntimeItemData;
    }
}
