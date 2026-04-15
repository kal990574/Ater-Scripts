using UnityEngine;
using UnityEngine.EventSystems;

public class SafeBoxButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SafeBoxInstance _owner;

    public void Bind(SafeBoxInstance owner)
    {
        _owner = owner;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_owner == null)
        {
            return;
        }

        _owner.ResetByButton();
    }
}