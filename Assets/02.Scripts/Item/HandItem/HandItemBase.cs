using UnityEngine;

//동기화 필요
public abstract class HandItemBase : MonoBehaviour,IBindApplier
{
    protected ItemBinderBase _binder;
    public ItemBinderBase Binder => _binder;
    
    public abstract void ApplyState(ItemBinderBase binder);
}
