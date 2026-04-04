using UnityEngine;

public abstract class StateApplierBase : MonoBehaviour, IStateApplier
{
    [SerializeField]protected RuntimeView _runtimeView;
    public IRuntimeView RuntimeView => _runtimeView;
    protected bool _isBind = false;
    
    private void Awake()
    {
        if (_runtimeView == null)
        {
            _runtimeView = GetComponentInParent<RuntimeView>();
        }
    }

    public abstract void ApplyState(RuntimeView binder);

    protected virtual bool CheckBindValid(string binderContext = "")
    {
        if (!_isBind)
        {
            Debug.LogError("[BindApplierBase] Binder is not initialized.", this);
            return false;
        }

        if (_runtimeView == null)
        {
            Debug.LogError("[BindApplierBase] InstanceView is missing.", this);
            return false;
        }

        if (_runtimeView.EnsureRuntimeData() == null)
        {
            Debug.LogError("[BindApplierBase] RuntimeData is missing.", this);
            return false;
        }

        return true;
    }
}
