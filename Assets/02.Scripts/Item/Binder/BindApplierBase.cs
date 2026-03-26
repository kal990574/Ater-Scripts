using UnityEngine;

public abstract class BindApplierBase : MonoBehaviour, IBindApplier
{
    protected InstanceView _binder;

    public InstanceView Binder => _binder;
    protected bool _isBind = false;
    public abstract void ApplyState(InstanceView binder);

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
