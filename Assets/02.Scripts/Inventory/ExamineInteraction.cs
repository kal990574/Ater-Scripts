using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExamineInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IScrollHandler, IPointerClickHandler
{
    public event Action<bool> OnDragChanged;
    public event Action<float> OnScrolled;
    public event Action<Vector2, RectTransform> OnClicked;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnDragChanged?.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnDragChanged?.Invoke(false);
    }

    public void OnScroll(PointerEventData eventData)
    {
        OnScrolled?.Invoke(eventData.scrollDelta.y);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnClicked?.Invoke(eventData.position, GetComponent<RectTransform>());
    }
}
