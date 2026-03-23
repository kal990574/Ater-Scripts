using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour,IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _indexText;
    [SerializeField] private Image _selected;

    private ItemData _itemData;
    private int _index;
    private Canvas _canvas;
    private GameObject _dragIcon;

    public ItemData ItemData => _itemData;
    public int Index => _index;

    public event Action<InventorySlotUI> OnClicked;
    public event Action<InventorySlotUI, InventorySlotUI> OnDropped;

    public void Setup(ItemData itemData, int index, Canvas canvas)
    {
        _itemData = itemData;
        _index = index;
        _canvas = canvas;
        Refresh();
    }

    private void Refresh()
    {
        _indexText.text = _index.ToString();
        if (_itemData == null)
        {
            _itemIcon.enabled = false;
            return;
        }

        _itemIcon.enabled = true;
        _itemIcon.sprite = _itemData.Icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemData == null) return;

        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(_canvas.transform, false);
        _dragIcon.transform.SetAsLastSibling();

        Image icon = _dragIcon.AddComponent<Image>();
        icon.sprite = _itemData.Icon;
        icon.raycastTarget = false;

        _itemIcon.enabled = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon == null) return;

        _dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
        {
            Destroy(_dragIcon);
            _dragIcon = null;
        }

        _itemIcon.enabled = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI dragged = eventData.pointerDrag.GetComponent<InventorySlotUI>();
        if (dragged == null) return;

        dragged.ClearDragIcon();
        OnDropped?.Invoke(dragged, this);
    }

    public void ClearDragIcon()
    {
        if (_dragIcon != null)
        {
            Destroy(_dragIcon);
            _dragIcon = null;
        }
        _itemIcon.enabled = true;
    }

    public void Select()
    {
        _selected.gameObject.SetActive(true);
    }

    public void Deselect()
    {
        _selected.gameObject.SetActive(false);
    }
}