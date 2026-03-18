using UnityEngine;
using UnityEngine.EventSystems;

public class InteractPoint : MonoBehaviour
{
    private Interactable _interactable;

    private void Awake()
    {
        _interactable = GetComponentInParent<Interactable>();
    }

    public void OnClick()
    {
        _interactable?.Interact();
    }
}