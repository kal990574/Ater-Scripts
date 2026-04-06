using UnityEngine;

public abstract class StateInteractable : UsableObject
{
    [Header("References")]
    [SerializeField] private RuntimeView _runtimeView;

    protected RuntimeView RuntimeViewComponent => _runtimeView;
    
    protected override void OnAwake()
    {
        base.OnAwake();

        if (_runtimeView == null)
        {
            _runtimeView = GetComponent<RuntimeView>();
        }
    }

    protected bool HasRuntimeState()
    {
        return ResolveRuntimeData()?.State != null;
    }

    protected RuntimeData ResolveRuntimeData()
    {
        if (_runtimeView != null && _runtimeView.RuntimeData != null)
        {
            return _runtimeView.RuntimeData;
        }

        return RuntimeData;
    }

    protected bool GetState(string key)
    {
        RuntimeData runtimeData = ResolveRuntimeData();
        if (runtimeData?.State == null || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return runtimeData.State.GetBool(key);
    }

    protected void SetState(string key, bool value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        if (_runtimeView != null)
        {
            _runtimeView.SetBoolState(key, value);
            return;
        }

        RuntimeData runtimeData = ResolveRuntimeData();
        if (runtimeData?.State == null)
        {
            return;
        }

        runtimeData.State.SetBool(key, value);
        RefreshRuntimeView();
    }

    protected bool ValidateStateKey(string key, string description, out string failureReason)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            failureReason = $"{description} is not configured.";
            return false;
        }

        failureReason = string.Empty;
        return true;
    }
}
