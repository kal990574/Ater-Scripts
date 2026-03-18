using UnityEngine;
using UnityEngine.EventSystems;

public class InteractPoint : MonoBehaviour
{
    private IInteractable _interactable;

    private void Awake()
    {
        _interactable = GetComponentInParent<IInteractable>();
    }

    public void OnClick()
    {
        _interactable?.Interact();
    }
}