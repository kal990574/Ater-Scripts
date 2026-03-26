using UnityEngine;

public class HandBox : BindApplierBase
{
    [SerializeField] private GameObject _rewardVisual;
    [SerializeField] private StateKeySO _openStateKey;
    [SerializeField] private StateKeySO _rewardCollectedStateKey;

    public override void ApplyState(InstanceView binder)
    {
        if (binder?.ItemInstance == null)
        {
            return;
        }

        bool isOpened = binder.ItemInstance.State.GetBool(_openStateKey);
        bool rewardCollected = binder.ItemInstance.State.GetBool(_rewardCollectedStateKey);

        if (_rewardVisual != null)
        {
            _rewardVisual.SetActive(isOpened && !rewardCollected);
        }
    }
}
