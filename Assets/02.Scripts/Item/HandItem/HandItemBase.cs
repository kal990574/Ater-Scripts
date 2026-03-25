using UnityEngine;

//동기화 필요
public abstract class HandItemBase : MonoBehaviour,IBindApplier
{
    protected ItemBinderBase _binder;
    protected bool _isBind = false;
    
    public ItemBinderBase Binder => _binder;
    
    public abstract void ApplyState(ItemBinderBase binder);
    
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

        if (!string.IsNullOrEmpty(binderContext) && _binder.ItemInstance.State.GetBool(binderContext))
        {
            Debug.LogError("이미 True상태임");
            return false;
        }

        return true;
    }
}
