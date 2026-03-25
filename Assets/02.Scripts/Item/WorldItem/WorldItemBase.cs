using UnityEngine;

public abstract class WorldItemBase : MonoBehaviour,IBindApplier
{
    protected ItemBinderBase _binder;
    
    public ItemBinderBase Binder => _binder;
    public abstract void ApplyState(ItemBinderBase binder);
}
