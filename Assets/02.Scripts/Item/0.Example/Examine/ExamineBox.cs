using UnityEngine;

public class ExamineBox : ExamineItemBase
{
    [SerializeField] private GameObject _interactObject;
    [SerializeField] private GameObject _keyObject;
    [SerializeField] private int  _rewardItemId;

    private bool _isOpened = false;
    private bool _isColleced = false;
    
    
    //해당 아이템이 생성될때 인스턴스의 스테이트 적용
    public override void ApplyState(ItemBinderBase binder)
    {
        Debug.Log("바인드 적용");
        _binder = binder;
        
        _isOpened = _binder.ItemInstance.State.GetBool(BinderContext.IS_OPEN);
        _isColleced = _binder.ItemInstance.State.GetBool(BinderContext.IS_REWARD_COLLECTED);

        //열려있지 않을때만 인터렉터블 포인트를 보이게 한다.
        if (_isOpened)
        {
            _interactObject.SetActive(false);
        }
        else
        {
            _interactObject.SetActive(true);
        }
        
        //상자가 열려있고 , 열쇄를 수집하지 않았다면 키를 보여준다.
        if (_isOpened && !_isColleced)
        {
            _keyObject.SetActive(true);
        }
        else
        {
            _keyObject.SetActive(false);
        }

        _isBind = true;
    }
  
    public void Open()
    {
        if (!CheckBindValid(BinderContext.IS_OPEN))
        {
            return;
        }

        _binder.ItemInstance.State.SetBool(BinderContext.IS_OPEN, true);
        _binder.RefreshView();
    }

    
    public void GetKey()
    {
        if (!CheckBindValid(BinderContext.IS_REWARD_COLLECTED))
        {
            return;
        }

        if (!_binder.InventoryManager.TryAddItem(_rewardItemId))
        {
            return;
        }

        _binder.ItemInstance.State.SetBool(BinderContext.IS_REWARD_COLLECTED, true);
        _binder.RefreshView();
    }
}
