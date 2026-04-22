using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class RuntimeInstanceManager : MonoBehaviour
{
    private static RuntimeInstanceManager _instance;
    public static RuntimeInstanceManager Instance => _instance;

    [Header("Reference")]
    [SerializeField] private ItemDataTableSO _itemDataTableSo;

    private RuntimeInstanceService _runtimeInstanceService;
    public ItemDataTableSO ItemDataTable => _itemDataTableSo;

    [ShowInInspector, ReadOnly, FoldoutGroup("Debug")]
    [DictionaryDrawerSettings(IsReadOnly = true, DisplayMode = DictionaryDisplayOptions.Foldout)]
    public IReadOnlyDictionary<string, RuntimeData> RuntimeInstances =>
        _runtimeInstanceService?.Instances;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _runtimeInstanceService = new RuntimeInstanceService(_itemDataTableSo);
            return;
        }

        if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetItemDataTable(ItemDataTableSO itemDataTableSo)
    {
        _itemDataTableSo = itemDataTableSo;
        _runtimeInstanceService?.SetItemDataTable(itemDataTableSo);
    }

    public RuntimeData CreateRuntimeData(string objectName = null, InteractState defaultState = null)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.CreateRuntimeData(objectName, defaultState);
    }

    public RuntimeData CreateRuntimeData(InteractState defaultState)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.CreateRuntimeData(defaultState);
    }

    public RuntimeData GetOrCreateRuntimeData(string instanceId, string objectName = null, InteractState defaultState = null)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.GetOrCreateRuntimeData(instanceId, objectName, defaultState);
    }

    public RuntimeData GetOrCreateRuntimeData(string instanceId, InteractState defaultState)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.GetOrCreateRuntimeData(instanceId, defaultState);
    }

    public RuntimeItemData CreateItemInstance(int itemId, string objectName = null)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.CreateInstance(itemId, objectName);
    }

    public RuntimeItemData CreateItemInstance(int itemId)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.CreateInstance(itemId);
    }

    public RuntimeItemData GetOrCreateItemInstance(string instanceId, int itemId, string objectName = null)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.GetOrCreateItemInstance(instanceId, itemId, objectName);
    }

    public RuntimeItemData GetOrCreateItemInstance(string instanceId, int itemId)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.GetOrCreateItemInstance(instanceId, itemId);
    }

    public bool RegisterInstance(RuntimeData runtimeData)
    {
        if (_runtimeInstanceService == null)
        {
            return false;
        }

        return _runtimeInstanceService.RegisterInstance(runtimeData);
    }

    public bool TryGetRuntimeData(string instanceId, out RuntimeData runtimeData)
    {
        runtimeData = null;

        if (_runtimeInstanceService == null)
        {
            return false;
        }

        return _runtimeInstanceService.TryGetRuntimeData(instanceId, out runtimeData);
    }

    public RuntimeData GetRuntimeData(string instanceId)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.GetRuntimeData(instanceId);
    }

    public bool TryGetItemInstance(string instanceId, out RuntimeItemData runtimeItemData)
    {
        runtimeItemData = null;

        if (_runtimeInstanceService == null)
        {
            return false;
        }

        return _runtimeInstanceService.TryGetItemInstance(instanceId, out runtimeItemData);
    }

    public RuntimeItemData GetItemInstance(string instanceId)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.GetItemInstance(instanceId);
    }

}
