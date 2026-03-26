using UnityEngine;

public class HandBox : BindApplierBase
{
    [SerializeField] private GameObject _rewardVisual;

    public override void ApplyState(ItemBinderBase binder)
    {
        if (binder?.ItemInstance == null)
        {
            return;
        }

        bool isOpened = binder.ItemInstance.State.GetBool(BinderContext.IS_OPEN);
        bool rewardCollected = binder.ItemInstance.State.GetBool(BinderContext.IS_REWARD_COLLECTED);

        if (_rewardVisual != null)
        {
            _rewardVisual.SetActive(isOpened && !rewardCollected);
        }
    }
}
