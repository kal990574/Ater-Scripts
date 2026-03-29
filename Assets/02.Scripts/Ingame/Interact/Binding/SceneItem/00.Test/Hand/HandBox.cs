using UnityEngine;

public class HandBox : StateApplierBase
{
    [SerializeField] private GameObject _rewardVisual;
    [SerializeField] private StateKeySO _openStateKey;
    [SerializeField] private StateKeySO _rewardCollectedStateKey;

    public override void ApplyState(RuntimeView binder)
    {
        if (binder?.RuntimeItemData == null)
        {
            return;
        }

        bool isOpened = binder.RuntimeItemData.State.GetBool(_openStateKey);
        bool rewardCollected = binder.RuntimeItemData.State.GetBool(_rewardCollectedStateKey);

        if (_rewardVisual != null)
        {
            _rewardVisual.SetActive(isOpened && !rewardCollected);
        }
    }
}
