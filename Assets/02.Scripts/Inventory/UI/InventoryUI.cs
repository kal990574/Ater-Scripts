using Unity.VisualScripting;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotUIPrefab;
    [SerializeField] private Transform _slotContainer;

    [SerializeField] private Canvas _canvas;

    private void Start()
    {
        InventoryManager.Instance.Inventory.OnItemAdded += OnItemAdded;
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.Inventory.OnItemAdded -= OnItemAdded;
    }

    private void OnItemAdded(InventorySlot slot)
    {
        GameObject slotObj = Instantiate(_slotUIPrefab, _slotContainer);
        InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
        slotUI.Setup(slot, InventoryManager.Instance.Inventory.Count, _canvas);
    }
}