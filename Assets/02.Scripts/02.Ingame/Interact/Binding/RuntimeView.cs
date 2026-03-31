using UnityEngine;

[DisallowMultipleComponent]
public class RuntimeView : MonoBehaviour, IRuntimeView
{
    [SerializeField, HideInInspector] private string _instanceId;
    [SerializeField] private bool _enableDebug = false;
    private InventoryManager _inventoryManager;

    public string InstanceId => _instanceId;
    public RuntimeData RuntimeData => ResolveRuntimeData();
    public RuntimeItemData RuntimeItemData => RuntimeData as RuntimeItemData;
    public InventoryManager InventoryManager => _inventoryManager;

    public virtual void Bind(string instanceId, InventoryManager inventoryManager)
    {
        SetInstanceId(instanceId);
        _inventoryManager = inventoryManager;
        LogBoundRuntimeData();
        PropagateRuntimeData();
        RefreshView();
    }

    public void SetInstanceId(string instanceId)
    {
        _instanceId = instanceId;
    }

    public RuntimeData EnsureRuntimeData()
    {
        RuntimeData runtimeData = ResolveRuntimeData();
        if (runtimeData == null)
        {
            Debug.LogError($"[{nameof(RuntimeView)}] RuntimeData is not registered for '{_instanceId}'.", this);
        }

        return runtimeData;
    }

    public RuntimeItemData EnsureItemInstance()
    {
        RuntimeItemData runtimeItemData = ResolveRuntimeData() as RuntimeItemData;
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(RuntimeView)}] RuntimeItemData is not registered for '{_instanceId}'.", this);
        }

        return runtimeItemData;
    }

    public void RefreshView()
    {
        if (ResolveRuntimeData() == null)
        {
            return;
        }

        IStateApplier[] binders = GetComponentsInChildren<IStateApplier>(true);
        if (binders == null || binders.Length == 0)
        {
            return;
        }
        foreach (IStateApplier binder in binders)
        {
            binder?.ApplyState(this);
        }
    }

    public bool SetBoolState(string key, bool value, bool refreshView = true)
    {
        RuntimeData runtimeData = EnsureRuntimeData();
        if (runtimeData?.State == null || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        runtimeData.State.SetBool(key, value);
        if (refreshView)
        {
            RefreshView();
        }

        return true;
    }

    public bool SetIntState(string key, int value, bool refreshView = true)
    {
        RuntimeData runtimeData = EnsureRuntimeData();
        if (runtimeData?.State == null || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        runtimeData.State.SetInt(key, value);
        if (refreshView)
        {
            RefreshView();
        }

        return true;
    }

    public bool SetStringState(string key, string value, bool refreshView = true)
    {
        RuntimeData runtimeData = EnsureRuntimeData();
        if (runtimeData?.State == null || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        runtimeData.State.SetString(key, value ?? string.Empty);
        if (refreshView)
        {
            RefreshView();
        }

        return true;
    }

    private InventoryManager ResolveInventoryManager()
    {
        if (_inventoryManager != null)
        {
            return _inventoryManager;
        }

        _inventoryManager = InventoryManager.Instance;
        return _inventoryManager;
    }

    private void PropagateRuntimeData()
    {
        INeedRuntimeData[] runtimeBindables = GetComponentsInChildren<INeedRuntimeData>(true);
        foreach (INeedRuntimeData runtimeBindable in runtimeBindables)
        {
            runtimeBindable?.SetRuntimeData(this);
        }
    }

    private RuntimeData ResolveRuntimeData()
    {
        InventoryManager inventoryManager = ResolveInventoryManager();
        if (inventoryManager == null || string.IsNullOrEmpty(_instanceId))
        {
            return null;
        }

        return inventoryManager.GetRuntimeData(_instanceId);
    }

    private void LogBoundRuntimeData()
    {
        if (!_enableDebug)
        {
            return;
        }
        RuntimeData runtimeData = ResolveRuntimeData();
        string runtimeType = runtimeData != null ? runtimeData.GetType().Name : "null";
        string itemInfo = runtimeData is RuntimeItemData runtimeItemData
            ? $", itemId={runtimeItemData.ItemId}, itemName={runtimeItemData.ItemName}"
            : string.Empty;
        string stateInfo = runtimeData?.State != null
            ? $", state={runtimeData.State.ToDebugString()}"
            : ", state=null";

        Debug.Log(
            $"[{nameof(RuntimeView)}] Bound '{gameObject.name}' to {runtimeType} (instanceId={_instanceId}{itemInfo}{stateInfo})",
            this);
    }
}
