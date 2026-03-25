using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_InventoryContainer : MonoBehaviour
{
    [SerializeField] private GameObject _slotUIPrefab;
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private Canvas _canvas;

    public event Action<int> OnSlotClicked;
    public event Action<int, int> OnSwapRequested;

    public void Refresh(IReadOnlyList<InventoryItemInstance> items)
    {
        List<GameObject> toDestroy = new List<GameObject>();

        foreach (Transform child in _slotContainer)
        {
            toDestroy.Add(child.gameObject);
        }

        foreach (GameObject obj in toDestroy)
        {
            obj.transform.SetParent(null);
        }

        for (int i = 0; i < items.Count; i++)
        {
            GameObject obj = Instantiate(_slotUIPrefab, _slotContainer);
            UI_InventorySlotItem slotItem = obj.GetComponent<UI_InventorySlotItem>();
            slotItem.Setup(items[i], i, _canvas);
            slotItem.OnClicked += HandleSlotClicked;
            slotItem.OnDropped += HandleSlotDropped;
        }

        foreach (GameObject obj in toDestroy)
        {
            Destroy(obj);
        }
    }

    public void SelectSlotAt(int index)
    {
        for (int i = 0; i < _slotContainer.childCount; i++)
        {
            _slotContainer.GetChild(i).GetComponent<UI_InventorySlotItem>().Deselect();
        }

        if (index < 0 || index >= _slotContainer.childCount)
        {
            return;
        }

        UI_InventorySlotItem slotItem = _slotContainer.GetChild(index).GetComponent<UI_InventorySlotItem>();
        if (slotItem == null)
        {
            return;
        }

        slotItem.Select();
    }

    private void HandleSlotClicked(UI_InventorySlotItem slotItem)
    {
        OnSlotClicked?.Invoke(slotItem.Index);
    }

    private void HandleSlotDropped(UI_InventorySlotItem dragged, UI_InventorySlotItem target)
    {
        OnSwapRequested?.Invoke(dragged.Index, target.Index);
    }
}
