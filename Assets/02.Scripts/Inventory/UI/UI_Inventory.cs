using System;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    //ItemViewer와 InventoryItmeUIContainer를 관리한다.
    [SerializeField] private UI_InventoryContainer uiInventoryContainer;
    [SerializeField] private UI_ItemViewer uiItemViewer;
    [SerializeField] private DetailViewInteraction _detailViewInteraction;

    private void Start()
    {
        InventoryManager.Instance.OnInventoryToggled += Show;
        Show(false);
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnDataChanged += Refresh;
        InventoryManager.Instance.OnSelectionChanged += HandleSelectionChanged;
        uiInventoryContainer.OnSlotClicked += HandleSlotClicked;
        uiInventoryContainer.OnSwapRequested += HandleSwapRequested;
        _detailViewInteraction.OnDragChanged += uiItemViewer.SetDragging;
        _detailViewInteraction.OnScrolled += uiItemViewer.Zoom;
        _detailViewInteraction.OnClicked += uiItemViewer.TryInteract;

        Refresh();
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnDataChanged -= Refresh;
        InventoryManager.Instance.OnSelectionChanged -= HandleSelectionChanged;
        uiInventoryContainer.OnSlotClicked -= HandleSlotClicked;
        uiInventoryContainer.OnSwapRequested -= HandleSwapRequested;
        _detailViewInteraction.OnDragChanged -= uiItemViewer.SetDragging;
        _detailViewInteraction.OnScrolled -= uiItemViewer.Zoom;
        _detailViewInteraction.OnClicked -= uiItemViewer.TryInteract;
        InventoryManager.Instance.ClearSelection();
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnInventoryToggled -= Show;
    }

    public void Refresh()
    {
        uiInventoryContainer.Refresh(InventoryManager.Instance.ReadonlyPlayerInventory);
        RestoreSelection();
    }

    private void RestoreSelection()
    {
        int index = InventoryManager.Instance.SelectedIndex;

        if (index < 0) return;

        if (index >= InventoryManager.Instance.ReadonlyPlayerInventory.Count) return;

        uiInventoryContainer.SelectSlotAt(index);
        uiItemViewer.ShowItem(InventoryManager.Instance.ReadonlyPlayerInventory[index]);
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
            uiItemViewer.Hide();
            return;
        }

        uiInventoryContainer.SelectSlotAt(index);
        uiItemViewer.ShowItem(InventoryManager.Instance.ReadonlyPlayerInventory[index]);
    }
    private void Show(bool isOn)
    {
        gameObject.SetActive(isOn);
    }   
}
