using UnityEngine;

public class ExamineBox : MonoBehaviour, IExamineInteractable, IBindApplier
{
    private const string OpenStateKey = "is_open";

    [SerializeField] private GameObject _rewardObject;

    private bool _isOpened;

    public void Interact(ExamineItemBinder binder)
    {
        if (_isOpened)
        {
            return;
        }

        _isOpened = true;
        if (_rewardObject != null)
        {
            _rewardObject.SetActive(true);
        }
        
        if (binder?.ItemInstance == null)
        {
            return;
        }

        if (binder.ItemInstance.State.GetBool(OpenStateKey))
        {
            return;
        }

        binder.ItemInstance.State.SetBool(OpenStateKey, true);
        binder.RefreshView();
    }

    public void ApplyState(ItemBinderBase binder)
    {
        bool isOpened = binder != null && binder.ItemInstance != null && binder.ItemInstance.State.GetBool(OpenStateKey);
        _isOpened = isOpened;

        if (_rewardObject != null)
        {
            _rewardObject.SetActive(isOpened);
        }
    }
}
