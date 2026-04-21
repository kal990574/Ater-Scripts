using System;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private UI_InventoryContainer _uiInventoryContainer;
    [SerializeField] private UI_InventoryItemViewer _uiInventoryItemViewer;
    [SerializeField] private ExamineInteraction _examineInteraction;

    [SerializeField] private int _selectedIndex = -1;

    private void Start()
    {
        InventoryManager.Instance.OnInventoryToggled += Show;
        Show(false);
    }

    private void OnEnable()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        InventoryManager.Instance.OnInventoryItemChanged += Refresh;
        InventoryManager.Instance.OnSelectionChanged += HandleSelectionChanged;
        _uiInventoryContainer.OnSlotClicked += HandleSlotClicked;
        _uiInventoryContainer.OnSwapRequested += HandleSwapRequested;

        _examineInteraction.OnDragChanged += _uiInventoryItemViewer.SetDragging;
        _examineInteraction.OnMoveDragChanged += _uiInventoryItemViewer.SetMoveDragging;
        _examineInteraction.OnScrolled += _uiInventoryItemViewer.Zoom;
        _examineInteraction.OnClicked += _uiInventoryItemViewer.TryInteract;

        Refresh();
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        InventoryManager.Instance.OnInventoryItemChanged -= Refresh;
        InventoryManager.Instance.OnSelectionChanged -= HandleSelectionChanged;
        _uiInventoryContainer.OnSlotClicked -= HandleSlotClicked;
        _uiInventoryContainer.OnSwapRequested -= HandleSwapRequested;

        _examineInteraction.OnDragChanged -= _uiInventoryItemViewer.SetDragging;
        _examineInteraction.OnMoveDragChanged -= _uiInventoryItemViewer.SetMoveDragging;
        _examineInteraction.OnScrolled -= _uiInventoryItemViewer.Zoom;
        _examineInteraction.OnClicked -= _uiInventoryItemViewer.TryInteract;

        _selectedIndex = -1;
        InventoryManager.Instance.ClearSelection();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }
        InventoryManager.Instance.OnInventoryToggled -= Show;
    }

    public void Refresh()
    {
        _uiInventoryContainer.Refresh(InventoryManager.Instance.ReadonlyPlayerInventoryInstanceIds);
        ApplySelection(InventoryManager.Instance.SelectedIndex, true);
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
        ApplySelection(index, false);
    }

    private void Show(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

    private void ApplySelection(int index, bool forceRefresh)
    {
        if (forceRefresh == false && _selectedIndex == index)
        {
            return;
        }

        _selectedIndex = index;
        _uiInventoryContainer.SelectSlotAt(index);

        string instanceId = InventoryManager.Instance.GetInventoryItemInstanceIdAt(index);
        if (string.IsNullOrEmpty(instanceId))
        {
            _uiInventoryItemViewer.Hide();
            return;
        }

        _uiInventoryItemViewer.ShowItem(instanceId);
    }
}
