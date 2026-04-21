using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExamineInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IScrollHandler, IPointerClickHandler
{
    public event Action<bool> OnDragChanged;
    public event Action<bool> OnMoveDragChanged;
    public event Action<float> OnScrolled;
    public event Action<Vector2, RectTransform> OnClicked;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnDragChanged?.Invoke(true);
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnMoveDragChanged?.Invoke(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnDragChanged?.Invoke(false);
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnMoveDragChanged?.Invoke(false);
        }
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
