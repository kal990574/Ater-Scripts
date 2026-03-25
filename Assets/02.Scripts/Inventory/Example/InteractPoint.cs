using UnityEngine;

public class InteractPoint : MonoBehaviour
{
    private IInteractableUI _interactableUI;
    private IUIViewItemInteractable _uiViewInteractable;
    private UIViewItemBinder _binder;

    private void Awake()
    {
        MonoBehaviour[] behaviours = GetComponentsInParent<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (_interactableUI == null && behaviour is IInteractableUI interactableUI)
            {
                _interactableUI = interactableUI;
            }

            if (_uiViewInteractable == null && behaviour is IUIViewItemInteractable uiViewInteractable)
            {
                _uiViewInteractable = uiViewInteractable;
            }
        }

        _binder = GetComponentInParent<UIViewItemBinder>();
    }

    public void OnClick()
    {
        if (_uiViewInteractable != null && _binder != null)
        {
            _uiViewInteractable.Interact(_binder);
            return;
        }

        _interactableUI?.Interact();
    }
}
