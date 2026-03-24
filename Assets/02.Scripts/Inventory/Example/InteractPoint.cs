using UnityEngine;
using UnityEngine.EventSystems;

public class InteractPoint : MonoBehaviour
{
    private IInteractableUI _interactableUI;

    private void Awake()
    {
        _interactableUI = GetComponentInParent<IInteractableUI>();
    }

    public void OnClick()
    {
        _interactableUI?.Interact();
    }
}