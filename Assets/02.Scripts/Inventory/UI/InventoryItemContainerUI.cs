using System;
using System.Collections.Generic;
using UnityEngine;

// 아이템UI 리프래시 (전체삭제/ 데이터리스트 받아서 순회하면서 생성하기)
public class InventoryItemContainerUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotUIPrefab;
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private Canvas _canvas;


    private InventorySlotUI _selectedSlotUI;

    public event Action<ItemData> OnSlotClicked;
    public event Action<int, int> OnSwapRequested;



    public void Refresh(IReadOnlyList<ItemData> items)
    {
        // 기존 슬롯을 컨테이너에서 즉시 분리
        List<GameObject> toDestroy = new List<GameObject>();

        foreach (Transform child in _slotContainer)
        {
            toDestroy.Add(child.gameObject);
        }

        foreach (GameObject obj in toDestroy)
        {
            obj.transform.SetParent(null); // 즉시 _slotContainer 계층에서 제거
        }

        _selectedSlotUI = null;

        // 새 슬롯 생성 (_slotContainer에는 새 슬롯만 존재)
        for (int i = 0; i < items.Count; i++)
        {
            GameObject obj = Instantiate(_slotUIPrefab, _slotContainer);
            InventorySlotUI slotUI = obj.GetComponent<InventorySlotUI>();
            slotUI.Setup(items[i], i, _canvas);
            slotUI.OnClicked += HandleSlotClicked;
            slotUI.OnDropped += HandleSlotDropped;
        }

        // 분리된 기존 슬롯 삭제
        foreach (GameObject obj in toDestroy)
        {
            Destroy(obj);
        }
    }

    private void HandleSlotClicked(InventorySlotUI slotUI)
    {
        if(_selectedSlotUI != null)
        {
            _selectedSlotUI.Deselect();
        }
        _selectedSlotUI = slotUI;
        _selectedSlotUI.Select();

        OnSlotClicked?.Invoke(slotUI.ItemData);
    }

    private void HandleSlotDropped(InventorySlotUI dragged, InventorySlotUI target)
    {
        OnSwapRequested?.Invoke(dragged.Index, target.Index);
    }


    public void SelectSlotAt(int index)
    {
        InventorySlotUI slotUI = _slotContainer.GetChild(index).GetComponent<InventorySlotUI>();

        if (slotUI == null) return;

        if (_selectedSlotUI != null)
        {
            _selectedSlotUI.Deselect();
        }

        _selectedSlotUI = slotUI;
        _selectedSlotUI.Select();
    }
}