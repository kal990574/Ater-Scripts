using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InventorySlotItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _indexText;
    [SerializeField] private Image _selected;

    private ItemInstance _itemInstance;
    private int _index;
    private Canvas _canvas;
    private GameObject _dragIcon;

    public ItemInstance ItemInstance => _itemInstance;
    public int Index => _index;

    public event Action<UI_InventorySlotItem> OnClicked;
    public event Action<UI_InventorySlotItem, UI_InventorySlotItem> OnDropped;

    public void Setup(ItemInstance itemInstance, int index, Canvas canvas)
    {
        _itemInstance = itemInstance;
        _index = index;
        _canvas = canvas;
        Refresh();
    }

    private void Refresh()
    {
        _indexText.text = _index.ToString();
        if (_itemInstance == null)
        {
            _itemIcon.enabled = false;
            return;
        }

        _itemIcon.enabled = true;
        _itemIcon.sprite = _itemInstance.Icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("아이템 클릭");
        OnClicked?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemInstance == null)
        {
            return;
        }

        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(_canvas.transform, false);
        _dragIcon.transform.SetAsLastSibling();

        Image icon = _dragIcon.AddComponent<Image>();
        icon.sprite = _itemInstance.Icon;
        icon.raycastTarget = false;

        _itemIcon.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon == null)
        {
            return;
        }

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
        UI_InventorySlotItem dragged = eventData.pointerDrag.GetComponent<UI_InventorySlotItem>();
        if (dragged == null)
        {
            return;
        }

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
