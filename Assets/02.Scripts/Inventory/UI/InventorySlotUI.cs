using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _indexText;
    [SerializeField] private Image _selected;

    private InventorySlot _slot;
    private Canvas _canvas;
    private GameObject _dragIcon;
    private InventoryUI _inventoryUI;

    public InventorySlot Slot => _slot;


    public void Setup(InventorySlot slot, int index, Canvas canvas, InventoryUI inventoryUI)
    {
        _slot = slot;
        _indexText.text = index.ToString();
        _canvas = canvas;
        _inventoryUI = inventoryUI;
        Refresh();
    }

    private void Refresh()
    {
        if (_slot.IsEmpty)
        {
            _itemIcon.enabled = false;
            return;
        }

        _itemIcon.enabled = true;
        _itemIcon.sprite = _slot.Item.Icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_slot.IsEmpty) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager.Instance.Inventory.RemoveItem(_slot);
            Destroy(gameObject);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"상세보기 : {_slot.Item} , 인덱스 : {_indexText.text}");
            _inventoryUI.SelectSlot(this);
            ItemViewer.Instance.ShowItem(_slot.Item);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_slot.IsEmpty) return;

        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(_canvas.transform, false);
        _dragIcon.transform.SetAsLastSibling();

        Image icon = _dragIcon.AddComponent<Image>();
        icon.sprite = _slot.Item.Icon;
        icon.raycastTarget = false;

        _itemIcon.enabled = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(_dragIcon == null) return;

        _dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_dragIcon != null)
        {
            Destroy(_dragIcon);
            _dragIcon = null;
        }

        _itemIcon.enabled = true;
        Refresh();
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI draggedSlotUI = eventData.pointerDrag.GetComponent<InventorySlotUI>();
        if (draggedSlotUI == null) return;

        InventoryManager.Instance.Inventory.SwapItem(draggedSlotUI.Slot, _slot);

        draggedSlotUI.Refresh();
        Refresh();

        //_inventoryUI.SelectSlot(this);
        //ItemViewer.Instance.ShowItem(_slot.Item);

        if (_inventoryUI.SelectedSlotUI == this)
        {
            ItemViewer.Instance.ShowItem(_slot.Item);
        }
        else if (_inventoryUI.SelectedSlotUI == draggedSlotUI)
        {
            ItemViewer.Instance.ShowItem(draggedSlotUI.Slot.Item);
        }
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