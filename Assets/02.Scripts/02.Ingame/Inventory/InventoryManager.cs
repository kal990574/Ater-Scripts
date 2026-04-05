using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;

    public static InventoryManager Instance => _instance;

    [Header("Debug/DontChange")]
    [SerializeField] private bool _isInventoryUIOn;

    private InventoryService _inventoryService;

    public IReadOnlyList<string> ReadonlyPlayerInventoryInstanceIds => _inventoryService.Items;
    public int Count => _inventoryService.Count;
    public int SelectedIndex => _inventoryService.SelectedIndex;

    public event Action<bool> OnInventoryToggled;
    public event Action OnInventoryItemChanged;
    public event Action<int> OnSelectionChanged;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _inventoryService = new InventoryService();
        _inventoryService.OnInventoryChanged += HandleInventoryChanged;
        _inventoryService.OnSelectionChanged += HandleSelectionChanged;
    }

    private void OnDestroy()
    {
        if (_inventoryService != null)
        {
            _inventoryService.OnInventoryChanged -= HandleInventoryChanged;
            _inventoryService.OnSelectionChanged -= HandleSelectionChanged;
        }

        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void ClearSelection()
    {
        _inventoryService.ClearSelection();
    }

    public void ToggleInventory()
    {
        _isInventoryUIOn = !_isInventoryUIOn;
        OnInventoryToggled?.Invoke(_isInventoryUIOn);
    }

    public bool SelectItem(int index)
    {
        return _inventoryService.Select(index);
    }

    public bool TryAddItem(RuntimeItemData runtimeItemData)
    {
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Tried to add a null item instance.", this);
            return false;
        }

        RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
        if (runtimeInstanceManager == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] {nameof(RuntimeInstanceManager)}.Instance is null.", this);
            return false;
        }

        runtimeInstanceManager.RegisterInstance(runtimeItemData);
        return _inventoryService.TryAdd(runtimeItemData.InstanceId);
    }

    public bool RemoveItem(int index)
    {
        return _inventoryService.RemoveAt(index);
    }

    public bool SwapItem(int index1, int index2)
    {
        return _inventoryService.Swap(index1, index2);
    }

    public int IndexOf(string instanceId)
    {
        return _inventoryService.IndexOf(instanceId);
    }

    public string GetInventoryItemInstanceIdAt(int index)
    {
        if (index < 0 || index >= _inventoryService.Count)
        {
            return null;
        }

        return _inventoryService.GetAt(index);
    }

    public bool TryGetInventoryItemInstanceIdAt(int index, out string instanceId)
    {
        instanceId = null;

        if (index < 0 || index >= _inventoryService.Count)
        {
            return false;
        }

        instanceId = _inventoryService.GetAt(index);
        return string.IsNullOrEmpty(instanceId) == false;
    }

    private void HandleInventoryChanged()
    {
        OnInventoryItemChanged?.Invoke();
    }

    private void HandleSelectionChanged(int selectedIndex)
    {
        OnSelectionChanged?.Invoke(selectedIndex);
    }
}
