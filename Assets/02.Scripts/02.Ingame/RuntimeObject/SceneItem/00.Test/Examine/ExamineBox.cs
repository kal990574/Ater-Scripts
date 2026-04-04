using UnityEngine;

public class ExamineBox : StateApplierBase
{
    [SerializeField] private GameObject _interactObject;
    [SerializeField] private GameObject _keyObject;
    [SerializeField] private int _rewardItemId;
    [SerializeField] private string _openStateKey = string.Empty;
    [SerializeField] private string _rewardCollectedStateKey = string.Empty;

    private bool _isOpened = false;
    private bool _isCollected = false;

    // 해당 아이템이 생성될 때 인스턴스의 스테이트를 적용한다.
    public override void ApplyState(RuntimeView binder)
    {
        Debug.Log("바인드 적용");

        _runtimeView = binder;

        if (_runtimeView == null || _runtimeView.RuntimeItemData == null)
        {
            _isBind = false;
            return;
        }

        _isOpened = _runtimeView.RuntimeItemData.State.GetBool(_openStateKey);
        _isCollected = _runtimeView.RuntimeItemData.State.GetBool(_rewardCollectedStateKey);

        // 열려 있지 않을 때만 인터랙트 오브젝트를 보이게 한다.
        if (_interactObject != null)
        {
            _interactObject.SetActive(_isOpened == false);
        }

        // 상자가 열려 있고 아직 보상을 획득하지 않았다면 키를 보여준다.
        if (_keyObject != null)
        {
            _keyObject.SetActive(_isOpened && _isCollected == false);
        }

        _isBind = true;
    }

    public void Open()
    {
        if (CheckBindValid() == false)
        {
            return;
        }

        _runtimeView.RuntimeItemData.State.SetBool(_openStateKey, true);
        _runtimeView.RefreshView();
    }

    public void GetKey()
    {
        if (CheckBindValid() == false)
        {
            return;
        }

        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
        InventoryManager inventoryManager = InventoryManager.Instance;

        if (runtimeInstanceManager == null)
        {
            Debug.LogError($"[{nameof(ExamineBox)}] {nameof(RuntimeInstanceManager)}.Instance is null.", this);
            return;
        }

        if (inventoryManager == null)
        {
            Debug.LogError($"[{nameof(ExamineBox)}] {nameof(InventoryManager)}.Instance is null.", this);
            return;
        }

        RuntimeItemData reward = runtimeInstanceManager.CreateItemInstance(_rewardItemId);
        if (reward == null)
        {
            Debug.LogError($"[{nameof(ExamineBox)}] Failed to create reward item. itemId={_rewardItemId}", this);
            return;
        }

        if (inventoryManager.TryAddItem(reward) == false)
        {
            Debug.LogWarning($"[{nameof(ExamineBox)}] Failed to add reward item to inventory. itemId={_rewardItemId}", this);
            return;
        }

        _runtimeView.RuntimeItemData.State.SetBool(_rewardCollectedStateKey, true);
        _runtimeView.RefreshView();
    }
}