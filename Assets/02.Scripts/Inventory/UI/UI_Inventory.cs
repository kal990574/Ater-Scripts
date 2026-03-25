using System;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    //ItemViewer와 InventoryItmeUIContainer를 관리한다.
    [SerializeField] private UI_InventoryContainer _uiInventoryContainer;
    [SerializeField] private UI_InventoryItemViewer _uiInventoryItemViewer;
    [SerializeField] private ExamineInteraction _examineInteraction;
    
    private int _selectedIndex = -1;
    private void Start()
    {
        InventoryManager.Instance.OnInventoryToggled += Show;
        Show(false);
    }

    private void OnEnable()
    {
        //임시
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (InventoryManager.Instance == null)
        {
            return;
        }
        
        InventoryManager.Instance.OnDataChanged += Refresh;
        InventoryManager.Instance.OnSelectionChanged += HandleSelectionChanged;
        _uiInventoryContainer.OnSlotClicked += HandleSlotClicked;
        _uiInventoryContainer.OnSwapRequested += HandleSwapRequested;
        
        _examineInteraction.OnDragChanged += _uiInventoryItemViewer.SetDragging;
        _examineInteraction.OnScrolled += _uiInventoryItemViewer.Zoom;
        _examineInteraction.OnClicked += _uiInventoryItemViewer.TryInteract;

        Refresh();
    }

    private void OnDisable()
    {
       
        //임시
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (InventoryManager.Instance == null)
        {
            return;
        }
        InventoryManager.Instance.OnDataChanged -= Refresh;
        InventoryManager.Instance.OnSelectionChanged -= HandleSelectionChanged;
        _uiInventoryContainer.OnSlotClicked -= HandleSlotClicked;
        _uiInventoryContainer.OnSwapRequested -= HandleSwapRequested;
        
        _examineInteraction.OnDragChanged -= _uiInventoryItemViewer.SetDragging;
        _examineInteraction.OnScrolled -= _uiInventoryItemViewer.Zoom;
        _examineInteraction.OnClicked -= _uiInventoryItemViewer.TryInteract;
        
        InventoryManager.Instance.ClearSelection();
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnInventoryToggled -= Show;
    }

    public void Refresh()
    {
        _uiInventoryContainer.Refresh(InventoryManager.Instance.ReadonlyPlayerInventory);
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
        if (_selectedIndex == index)
        {
            return;
        }
        
        _uiInventoryContainer.SelectSlotAt(index);
        _uiInventoryItemViewer.ShowItem(InventoryManager.Instance.ReadonlyPlayerInventory[index]);
    }
    
    private void Show(bool isOn)
    {
        gameObject.SetActive(isOn);
    }   
}
