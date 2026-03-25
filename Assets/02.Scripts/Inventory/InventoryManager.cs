using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    public static InventoryManager Instance => _instance;
    
    [Header("Reference")]
    [SerializeField] private ItemDataTable _table;
    [SerializeField] private List<ItemInstance> _playerInventory = new();
    
    [Header("Debug/DontChange")]
    [SerializeField] private bool _isInventoryUIOn = false;
    [SerializeField] private int _selectedIndex = -1;

    private readonly Dictionary<string, GameObject> _examineItemCache = new();
    private GameObject _currentExamineItemObject;
    
    public IReadOnlyList<ItemInstance> ReadonlyPlayerInventory => _playerInventory;
    public int Count => _playerInventory.Count;
    public int SelectedIndex => _selectedIndex;
    
    public event Action<bool> OnInventoryToggled;
    public event Action OnDataChanged;
    public event Action<int> OnSelectionChanged;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        
        TryAddItem(1);
    }
    
    #region Managing Inventory
    public void ClearSelection()
    {
        _selectedIndex = -1;
        OnSelectionChanged?.Invoke(_selectedIndex);
    }

    public void ToggleInventory()
    {
        _isInventoryUIOn = !_isInventoryUIOn;
        OnInventoryToggled?.Invoke(_isInventoryUIOn);
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= _playerInventory.Count) return;
        _selectedIndex = index;
        OnSelectionChanged?.Invoke(index);
    }

    public bool TryAddItem(int item)
    {
        ItemInstance newItem = CreateItemInstance(item);
        if (newItem == null)
        {
            return false;
        }

        return TryAddItem(newItem);
    }

    public ItemInstance CreateItemInstance(int itemId)
    {
        return _table.CreateInstance(itemId);
    }

    public bool TryAddItem(ItemInstance itemInstance)
    {
        if (itemInstance == null)
        {
            return false;
        }

        _playerInventory.Add(itemInstance);
        OnDataChanged?.Invoke();
        return true;
    }

    public void RemoveItem(int index)
    {
        _playerInventory.RemoveAt(index);

        if (_playerInventory.Count == 0)
        {
            _selectedIndex = -1;
        }

        else if (_selectedIndex > index)
        {
            _selectedIndex--;
        }
        else if (_selectedIndex >= _playerInventory.Count)
        {
            _selectedIndex = _playerInventory.Count - 1;
        }

        OnDataChanged?.Invoke();
        OnSelectionChanged?.Invoke(_selectedIndex);
    }

    public bool HasItem(int itemId)
    {
        foreach (ItemInstance item in _playerInventory)
        {
            if (item.ItemId == itemId) return true;
        }

        return false;
    }

    public void SwapItem(int index1, int index2)
    {
        var temp = _playerInventory[index1];
        _playerInventory[index1] = _playerInventory[index2];
        _playerInventory[index2] = temp;

        OnDataChanged?.Invoke();

        if (_selectedIndex == -1)
        {
            SelectItem(index2);
        }
        else if (_selectedIndex == index1 || _selectedIndex == index2)
        {
            SelectItem(_selectedIndex);
        }
    }

    #endregion
    
    #region Examine
    public GameObject ShowExamineItem(ItemInstance itemInstance, Transform itemRoot)
    {
        HideExamineItem();
        if (itemInstance == null || itemRoot == null || itemInstance.ExaminePrefab == null)
        {
            return null;
        }
        
        if (_examineItemCache.TryGetValue(itemInstance.InstanceId, out GameObject cached))
        {
            cached.SetActive(true);
            BindItemInstance(cached, itemInstance);
            _currentExamineItemObject = cached;
            return cached;
        }
        
        GameObject examineObject = Instantiate(itemInstance.ExaminePrefab, itemRoot, false);
        examineObject.transform.localPosition = Vector3.zero;
        SetLayerRecursively(examineObject, itemRoot.gameObject.layer);
        BindItemInstance(examineObject, itemInstance);

        _examineItemCache[itemInstance.InstanceId] = examineObject;
        _currentExamineItemObject = examineObject;
        return examineObject;
    }

    public void HideExamineItem()
    {
        if (_currentExamineItemObject == null)
        {
            return;
        }

        _currentExamineItemObject.SetActive(false);
        _currentExamineItemObject = null;
    }
    
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    #endregion

    public GameObject CreateWorldItem(ItemInstance itemInstance, Transform itemRoot)
    {
        GameObject item = Instantiate(itemInstance.WorldPrefab, itemRoot, false);
        item.transform.localPosition = Vector3.zero;
        SetLayerRecursively(item, itemRoot.gameObject.layer);
        BindItemInstance(item, itemInstance);
        return item;
    }
    
    public GameObject CreateHandItem(ItemInstance itemInstance, Transform itemRoot)
    {
        GameObject item = Instantiate(itemInstance.HandPrefab, itemRoot, false);
        item.transform.localPosition = Vector3.zero;
        SetLayerRecursively(item, itemRoot.gameObject.layer);
        BindItemInstance(item, itemInstance);
        return item;
    }
    
    private void BindItemInstance(GameObject itemObject, ItemInstance itemInstance)
    {
        ItemBinderBase binder = itemObject.GetComponent<ItemBinderBase>();
        if (binder == null)
        {
            binder = itemObject.AddComponent<ItemBinderBase>();
        }

        binder.Bind(itemInstance, this);
    }
}
