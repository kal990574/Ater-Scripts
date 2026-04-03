using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExamineInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IScrollHandler, IPointerClickHandler
{
    public event Action<bool> OnDragChanged;
    public event Action<float> OnScrolled;
    public event Action<Vector2, RectTransform> OnClicked;

    private GameEventPublisher _publisher;

    private void Awake()
    {
        _publisher = new GameEventPublisher();
        _publisher.SetSource(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnDragChanged?.Invoke(true);
        _publisher.TryPublish(ctx => new ItemDraggedRawEvent(ctx));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnDragChanged?.Invoke(false);
    }

    public void OnScroll(PointerEventData eventData)
    {
        OnScrolled?.Invoke(eventData.scrollDelta.y);
        _publisher.TryPublish(ctx => new ItemScrolledRawEvent(ctx));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OnClicked?.Invoke(eventData.position, GetComponent<RectTransform>());
    }
}
