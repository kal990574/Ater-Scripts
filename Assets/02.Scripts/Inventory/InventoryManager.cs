using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    public static InventoryManager Instance => _instance;
    
    [Header("Reference")]
    [SerializeField] private ItemDataTable _table;
    [SerializeField] private List<ItemInstance> _playerInventory = new();
    [SerializeField] private Transform _cachedExamineRoot;
    [SerializeField] private Transform _cachedHandRoot;
    
    [Header("Debug/DontChange")]
    [SerializeField] private bool _isInventoryUIOn = false;
    [SerializeField] private int _selectedIndex = -1;

    private readonly Dictionary<string, GameObject> _examineItemCache = new();
    private readonly Dictionary<string, GameObject> _handItemCache = new();
    private GameObject _currentExamineItemObject;
    private GameObject _currentHandItemObject;
    
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
        Debug.LogError($"[{nameof(InventoryManager)}] TryAddItem(int) is not allowed. Create an ItemInstance first for item id {item}.", this);
        return false;
    }

    public ItemInstance CreateItemInstance(int itemId)
    {
        if (_table == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] {nameof(ItemDataTable)} reference is missing.", this);
            return null;
        }

        ItemInstance itemInstance = _table.CreateInstance(itemId);
        if (itemInstance == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Failed to create ItemInstance for item id {itemId}.", this);
        }

        return itemInstance;
    }

    public bool TryAddItem(ItemInstance itemInstance)
    {
        if (itemInstance == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Tried to add a null ItemInstance to inventory.", this);
            return false;
        }

        _playerInventory.Add(itemInstance);
        CacheOwnedItem(itemInstance);
        OnDataChanged?.Invoke();
        return true;
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= _playerInventory.Count)
        {
            return;
        }

        ItemInstance removedItem = _playerInventory[index];
        DestroyOwnedItemCache(removedItem);
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
        if (index1 < 0 || index1 >= _playerInventory.Count || index2 < 0 || index2 >= _playerInventory.Count)
        {
            return;
        }

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
            OnSelectionChanged?.Invoke(_selectedIndex);
        }
    }
    #endregion
    
    #region Examine
    public GameObject ShowExamineItem(ItemInstance itemInstance)
    {
        HideExamineItem();
        if (itemInstance == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Cannot show examine item because ItemInstance is null.", this);
            return null;
        }

        GameObject examineObject = GetOrCreateCachedExamineItem(itemInstance);
        if (examineObject == null)
        {
            return null;
        }

        SetLayerRecursively(examineObject, ResolveCacheRoot(_cachedExamineRoot).gameObject.layer);
        examineObject.SetActive(true);
        _currentExamineItemObject = examineObject;
        return examineObject;
    }

    public void HideExamineItem()
    {
        if (_currentExamineItemObject == null)
        {
            return;
        }

        MoveToCacheRoot(_currentExamineItemObject, _cachedExamineRoot);
        _currentExamineItemObject.SetActive(false);
        _currentExamineItemObject = null;
    }
    #endregion

    #region World Object
    public GameObject CreateWorldItem(ItemInstance itemInstance, Transform itemRoot)
    {
        if (itemInstance == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Cannot create a world item from a null ItemInstance.", this);
            return null;
        }

        if (itemInstance.WorldPrefab == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Item {itemInstance.ItemName} has no WorldPrefab.", this);
            return null;
        }

        GameObject item = Instantiate(itemInstance.WorldPrefab, itemRoot, false);
        if (itemRoot != null)
        {
            item.transform.localPosition = Vector3.zero;
            SetLayerRecursively(item, itemRoot.gameObject.layer);
        }

        BindItemInstance(item, itemInstance);
        return item;
    }
    #endregion
    
    #region Hand Object
    public GameObject ShowHandItem(ItemInstance itemInstance)
    {
        HideHandItem();
        if (itemInstance == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Cannot show hand item because ItemInstance is null.", this);
            return null;
        }

        GameObject handObject = GetOrCreateCachedHandItem(itemInstance);
        if (handObject == null)
        {
            return null;
        }

        SetLayerRecursively(handObject, ResolveCacheRoot(_cachedHandRoot).gameObject.layer);
        handObject.SetActive(true);
        _currentHandItemObject = handObject;
        return handObject;
    }

    public void HideHandItem()
    {
        if (_currentHandItemObject == null)
        {
            return;
        }

        MoveToCacheRoot(_currentHandItemObject, _cachedHandRoot);
        _currentHandItemObject.SetActive(false);
        _currentHandItemObject = null;
    }
    #endregion

    #region Cache Helpers
    private void CacheOwnedItem(ItemInstance itemInstance)
    {
        GetOrCreateCachedExamineItem(itemInstance);
        GetOrCreateCachedHandItem(itemInstance);
    }

    private GameObject GetOrCreateCachedExamineItem(ItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.ExaminePrefab == null)
        {
            return null;
        }

        if (_examineItemCache.TryGetValue(itemInstance.InstanceId, out GameObject cached) && cached != null)
        {
            BindItemInstance(cached, itemInstance);
            return cached;
        }

        Transform cacheRoot = ResolveCacheRoot(_cachedExamineRoot);
        GameObject examineObject = Instantiate(itemInstance.ExaminePrefab, cacheRoot, false);
        examineObject.SetActive(false);
        BindItemInstance(examineObject, itemInstance);
        _examineItemCache[itemInstance.InstanceId] = examineObject;
        return examineObject;
    }

    private GameObject GetOrCreateCachedHandItem(ItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.HandPrefab == null)
        {
            return null;
        }

        if (_handItemCache.TryGetValue(itemInstance.InstanceId, out GameObject cached) && cached != null)
        {
            BindItemInstance(cached, itemInstance);
            return cached;
        }

        Transform cacheRoot = ResolveCacheRoot(_cachedHandRoot);
        GameObject handObject = Instantiate(itemInstance.HandPrefab, cacheRoot, false);
        handObject.SetActive(false);
        BindItemInstance(handObject, itemInstance);
        _handItemCache[itemInstance.InstanceId] = handObject;
        return handObject;
    }

    private void DestroyOwnedItemCache(ItemInstance itemInstance)
    {
        if (itemInstance == null)
        {
            return;
        }

        if (_examineItemCache.Remove(itemInstance.InstanceId, out GameObject examineObject) && examineObject != null)
        {
            if (_currentExamineItemObject == examineObject)
            {
                _currentExamineItemObject = null;
            }

            Destroy(examineObject);
        }

        if (_handItemCache.Remove(itemInstance.InstanceId, out GameObject handObject) && handObject != null)
        {
            if (_currentHandItemObject == handObject)
            {
                _currentHandItemObject = null;
            }

            Destroy(handObject);
        }
    }
    

    private void MoveToCacheRoot(GameObject itemObject, Transform cacheRoot)
    {
        itemObject.transform.SetParent(ResolveCacheRoot(cacheRoot), false);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.identity;
        itemObject.transform.localScale = Vector3.one;
    }
    #endregion

    #region Binding Helpers
    private void BindItemInstance(GameObject itemObject, ItemInstance itemInstance)
    {
        if (!itemObject.TryGetComponent(out IItemInstance binder))
        {
            Debug.Log("[InventoryManager] Can't Find ItemInstance ");
            return;
        }

        binder.Bind(itemInstance, this);
    }
    
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private Transform ResolveCacheRoot(Transform cacheRoot)
    {
        return cacheRoot != null ? cacheRoot : transform;
    }
    #endregion
}
