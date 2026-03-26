using UnityEngine;

//바인드 어플라이어는 스테이트를 통해 해당 오브젝트의 상태를 변경하는 역할을 한다.
public abstract class BindApplierBase : MonoBehaviour,IBindApplier
{
    protected InstanceView _binder;
    
    public InstanceView Binder => _binder;
    protected bool _isBind = false;
    public abstract void ApplyState(InstanceView binder);
    
    protected virtual bool CheckBindValid(string binderContext = "")
    {
        if (!_isBind)
        {
            Debug.LogError("현재 바인드 되지 않음.");
            return false;
        }

        if (_binder?.ItemInstance == null)
        {
            Debug.LogError("바인더의 인스턴스가 없음");
            return false;
        }

        return true;
    }
}
