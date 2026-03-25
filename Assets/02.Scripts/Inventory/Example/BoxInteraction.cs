using UnityEngine;

public class BoxInteraction : MonoBehaviour, IInteractableUI, IInventoryItemViewInteractable, IInventoryItemViewStateHandler
{
    private const string OpenStateKey = "is_open";

    [SerializeField] private GameObject _rewardObject;

    private bool _isOpened;

    public void Interact()
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
    }

    public void Interact(InventoryItemViewContext context)
    {
        if (context?.ItemInstance == null)
        {
            return;
        }

        if (context.ItemInstance.State.GetBool(OpenStateKey))
        {
            return;
        }

        context.ItemInstance.State.SetBool(OpenStateKey, true);
        context.RefreshView();
    }

    public void ApplyState(InventoryItemViewContext context)
    {
        bool isOpened = context != null && context.ItemInstance != null && context.ItemInstance.State.GetBool(OpenStateKey);
        _isOpened = isOpened;

        if (_rewardObject != null)
        {
            _rewardObject.SetActive(isOpened);
        }
    }
}
