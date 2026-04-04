using UnityEngine;

public class RuntimeInstanceManager : MonoBehaviour
{
    private static RuntimeInstanceManager _instance;

    public static RuntimeInstanceManager Instance => _instance;

    [Header("Reference")]
    [SerializeField] private ItemDataTableSO _itemDataTableSo;

    private RuntimeInstanceService _runtimeInstanceService;

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

    public RuntimeData CreateRuntimeData(InteractState defaultState = null)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.CreateRuntimeData(defaultState);
    }

    public RuntimeData GetOrCreateRuntimeData(string instanceId, InteractState defaultState = null)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.GetOrCreateRuntimeData(instanceId, defaultState);
    }

    public RuntimeItemData CreateItemInstance(int itemId)
    {
        if (_runtimeInstanceService == null)
        {
            return null;
        }

        return _runtimeInstanceService.CreateInstance(itemId);
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