using UnityEngine;

public abstract class StateApplierBase : MonoBehaviour, IStateApplier
{
    protected RuntimeView _binder;

    public RuntimeView Binder => _binder;
    protected bool _isBind = false;
    public abstract void ApplyState(RuntimeView binder);

    protected virtual bool CheckBindValid(string binderContext = "")
    {
        if (!_isBind)
        {
            Debug.LogError("[BindApplierBase] Binder is not initialized.", this);
            return false;
        }

        if (_binder == null)
        {
            Debug.LogError("[BindApplierBase] InstanceView is missing.", this);
            return false;
        }

        if (_binder.EnsureItemInstance() == null)
        {
            Debug.LogError("[BindApplierBase] ItemInstance is missing.", this);
            return false;
        }

        return true;
    }
}
