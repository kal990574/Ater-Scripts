using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _indexText;

    private InventorySlot _slot;

    public void Setup(InventorySlot slot, int index)
    {
        _slot = slot;
        _indexText.text = index.ToString();
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
        }
    }
}