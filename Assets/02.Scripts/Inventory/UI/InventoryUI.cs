using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    //ItemViewer와 InventoryItmeUIContainer를 관리한다.
    [SerializeField] private InventoryItemContainerUI _inventoryItemContainer;
    [SerializeField] private ItemViewerUI _itemViewer;
    [SerializeField] private DetailViewInteraction _detailViewInteraction;

    private void Awake()
    {
        InventoryManager.Instance.OnInventoryToggled += SetInventoryActive;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnDataChanged += Refresh;
        InventoryManager.Instance.OnSelectionChanged += HandleSelectionChanged;
        _inventoryItemContainer.OnSlotClicked += HandleSlotClicked;
        _inventoryItemContainer.OnSwapRequested += HandleSwapRequested;
        _detailViewInteraction.OnDragChanged += _itemViewer.SetDragging;
        _detailViewInteraction.OnScrolled += _itemViewer.Zoom;
        _detailViewInteraction.OnClicked += _itemViewer.TryInteract;

        Refresh();
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnDataChanged -= Refresh;
        InventoryManager.Instance.OnSelectionChanged -= HandleSelectionChanged;
        _inventoryItemContainer.OnSlotClicked -= HandleSlotClicked;
        _inventoryItemContainer.OnSwapRequested -= HandleSwapRequested;
        _detailViewInteraction.OnDragChanged -= _itemViewer.SetDragging;
        _detailViewInteraction.OnScrolled -= _itemViewer.Zoom;
        _detailViewInteraction.OnClicked -= _itemViewer.TryInteract;

        InventoryManager.Instance.ClearSelection();
    }

    public void Refresh()
    {
        _inventoryItemContainer.Refresh(InventoryManager.Instance.ReadonlyPlayerInventory);
        RestoreSelection();
    }

    private void RestoreSelection()
    {
        int index = InventoryManager.Instance.SelectedIndex;

        if (index < 0) return;

        if (index >= InventoryManager.Instance.ReadonlyPlayerInventory.Count) return;

        _inventoryItemContainer.SelectSlotAt(index);
        _itemViewer.ShowItem(InventoryManager.Instance.ReadonlyPlayerInventory[index]);
    }



    private void HandleSlotClicked(int index)
    {
        InventoryManager.Instance.SelectItem(index);
    }

    private void HandleSwapRequested(int index1, int index2)
    {
        InventoryManager.Instance.SwapItem(index1, index2);
    }

    private void HandleSelectionChanged(int index)
    {
        if(index < 0)
        {
            _itemViewer.Hide();
            return;
        }

        _inventoryItemContainer.SelectSlotAt(index);
        _itemViewer.ShowItem(InventoryManager.Instance.ReadonlyPlayerInventory[index]);
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnInventoryToggled -= SetInventoryActive;

    }

    private void SetInventoryActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

}
