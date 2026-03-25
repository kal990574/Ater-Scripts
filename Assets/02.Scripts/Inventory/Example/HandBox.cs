using UnityEngine;

public class HandBox : MonoBehaviour, IItemInstanceStateHandler
{
    private const string OpenStateKey = "is_open";
    private const string RewardCollectedStateKey = "reward_collected";

    [SerializeField] private GameObject _closedVisual;
    [SerializeField] private GameObject _openedVisual;
    [SerializeField] private GameObject _rewardVisual;

    public void ApplyState(ItemInstanceBinderBase binder)
    {
        if (binder?.ItemInstance == null)
        {
            return;
        }

        bool isOpened = binder.ItemInstance.State.GetBool(OpenStateKey);
        bool rewardCollected = binder.ItemInstance.State.GetBool(RewardCollectedStateKey);

        if (_closedVisual != null)
        {
            _closedVisual.SetActive(!isOpened);
        }

        if (_openedVisual != null)
        {
            _openedVisual.SetActive(isOpened);
        }

        if (_rewardVisual != null)
        {
            _rewardVisual.SetActive(isOpened && !rewardCollected);
        }
    }
}
