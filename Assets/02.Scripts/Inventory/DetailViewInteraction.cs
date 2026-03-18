using UnityEngine;
using UnityEngine.EventSystems;

public class DetailViewInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IScrollHandler, IPointerClickHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ItemViewer.Instance.SetDragging(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ItemViewer.Instance.SetDragging(false);
    }

    public void OnScroll(PointerEventData eventData)
    {
        ItemViewer.Instance.Zoom(eventData.scrollDelta.y);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        ItemViewer.Instance.TryInteract(eventData.position, GetComponent<RectTransform>());
    }
}
