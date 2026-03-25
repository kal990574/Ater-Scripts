using UnityEngine;

public class InteractPoint : MonoBehaviour
{
    private IInteractableUI _interactableUI;
    private IInventoryItemViewInteractable _inventoryInteractable;
    private InventoryItemViewContext _context;

    private void Awake()
    {
        MonoBehaviour[] behaviours = GetComponentsInParent<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (_interactableUI == null && behaviour is IInteractableUI interactableUI)
            {
                _interactableUI = interactableUI;
            }

            if (_inventoryInteractable == null && behaviour is IInventoryItemViewInteractable inventoryInteractable)
            {
                _inventoryInteractable = inventoryInteractable;
            }
        }

        _context = GetComponentInParent<InventoryItemViewContext>();
    }

    public void OnClick()
    {
        if (_inventoryInteractable != null && _context != null)
        {
            _inventoryInteractable.Interact(_context);
            return;
        }

        _interactableUI?.Interact();
    }
}
