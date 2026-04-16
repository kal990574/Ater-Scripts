using Sirenix.OdinInspector;
using UnityEngine;

[DisallowMultipleComponent]
public class RuntimeView : MonoBehaviour, IRuntimeView
{
    [SerializeField, HideInInspector] private string _instanceId;
    [SerializeField] private bool _enableDebug = false;

    private RuntimeData _runtimeData;

    public string InstanceId => _instanceId;
    public RuntimeData RuntimeData => _runtimeData;
    public RuntimeItemData RuntimeItemData => _runtimeData as RuntimeItemData;

    [FoldoutGroup("Debug"), ShowInInspector, ReadOnly, LabelText("Bound Runtime Data")]
    private RuntimeData DebugBoundRuntimeData => _runtimeData;

    [FoldoutGroup("Debug"), ShowInInspector, ReadOnly, LabelText("Stored Runtime Data")]
    private RuntimeData DebugStoredRuntimeData => ResolveStoredRuntimeData();

    [FoldoutGroup("Debug"), ShowInInspector, ReadOnly, LabelText("Stored State")]
    [MultiLineProperty(3)]
    private string DebugStoredState => ResolveDebugState();

    [FoldoutGroup("Debug"), ShowInInspector, ReadOnly, LabelText("Uses Stored Reference")]
    private bool DebugUsesStoredReference
    {
        get
        {
            RuntimeData storedRuntimeData = ResolveStoredRuntimeData();
            return storedRuntimeData != null && ReferenceEquals(_runtimeData, storedRuntimeData);
        }
    }

    public virtual void Bind(RuntimeData runtimeData)
    {
        _runtimeData = runtimeData;
        _instanceId = runtimeData != null ? runtimeData.InstanceId : null;

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
        if (_runtimeData == null)
        {
            Debug.LogError($"[{nameof(RuntimeView)}] RuntimeData is null for '{_instanceId}'.", this);
        }

        return _runtimeData;
    }

    public RuntimeItemData EnsureItemInstance()
    {
        RuntimeItemData runtimeItemData = _runtimeData as RuntimeItemData;
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(RuntimeView)}] RuntimeItemData is null for '{_instanceId}'.", this);
        }

        return runtimeItemData;
    }

    public void RefreshView()
    {
        if (_runtimeData == null)
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

    private void PropagateRuntimeData()
    {
        IRuntimeDataConsumer[] runtimeBindables = GetComponentsInChildren<IRuntimeDataConsumer>(true);
        foreach (IRuntimeDataConsumer runtimeBindable in runtimeBindables)
        {
            runtimeBindable?.SetRuntimeData(this);
        }
    }

    private void LogBoundRuntimeData()
    {
        if (_enableDebug == false)
        {
            return;
        }

        string runtimeType = _runtimeData != null ? _runtimeData.GetType().Name : "null";
        string itemInfo = _runtimeData is RuntimeItemData runtimeItemData
            ? $", itemId={runtimeItemData.ItemId}, itemName={runtimeItemData.ItemName}"
            : string.Empty;
        string stateInfo = _runtimeData?.State != null
            ? $", state={_runtimeData.State.ToDebugString()}"
            : ", state=null";

        Debug.Log(
            $"[{nameof(RuntimeView)}] Bound '{gameObject.name}' to {runtimeType} (instanceId={_instanceId}{itemInfo}{stateInfo})",
            this);
    }

    private RuntimeData ResolveStoredRuntimeData()
    {
        if (string.IsNullOrWhiteSpace(_instanceId))
        {
            return null;
        }

        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
        if (runtimeInstanceManager == null)
        {
            return _runtimeData;
        }

        return runtimeInstanceManager.GetRuntimeData(_instanceId) ?? _runtimeData;
    }

    private string ResolveDebugState()
    {
        RuntimeData runtimeData = ResolveStoredRuntimeData();
        if (runtimeData?.State != null)
        {
            return runtimeData.State.ToDebugString();
        }

        return "null";
    }
}
