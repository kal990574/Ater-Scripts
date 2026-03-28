using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private const string DefaultSaveKey = "Ater.Inventory.Save";

    private static InventoryManager _instance;
    public static InventoryManager Instance => _instance;

    [Header("Reference")]
    [SerializeField] private ItemDataTable _table;
    [SerializeField] private Transform _cachedExamineRoot;
    [SerializeField] private Transform _cachedHandRoot;

    [Header("Save")]
    [SerializeField] private bool _loadFromPlayerPrefsOnAwake = true;
    [SerializeField] private string _playerPrefsSaveKey = DefaultSaveKey;

    [Header("Debug/DontChange")]
    [SerializeField] private bool _isInventoryUIOn = false;
    [SerializeField] private int _selectedIndex = -1;

    private readonly Dictionary<string, PersistentSceneItem> _registeredSceneItems = new();
    private readonly Dictionary<string, SceneItemSaveData> _sceneItemStates = new();

    private InventoryService _inventoryService;
    private RuntimeInstanceService _instanceInstanceService;
    private HandService _handService;
    private ExamineService _examineService;
    private WorldService _worldService;
    private ItemFactory _itemFactory;

    public IReadOnlyList<string> ReadonlyPlayerInventoryInstanceIds => _inventoryService.Items;
    public int Count => _inventoryService.Count;
    public int SelectedIndex => _inventoryService.SelectedIndex;
    public string CurrentHandItemInstanceId => _handService.EquippedInstanceId;
    public ItemInstanceData CurrentHandItem => GetItemInstance(_handService.EquippedInstanceId);

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
            return;
        }

        _instanceInstanceService = new RuntimeInstanceService(_table);
        _instanceInstanceService.SetItemDataTable(_table);
        _inventoryService = new InventoryService();
        _itemFactory = new ItemFactory(_instanceInstanceService, this);
        _handService = new HandService(_itemFactory, ResolveRoot(_cachedHandRoot));
        _examineService = new ExamineService(_itemFactory, ResolveRoot(_cachedExamineRoot));
        _worldService = new WorldService(_itemFactory);
        _inventoryService.OnInventoryChanged += HandleInventoryChanged;
        _inventoryService.OnSelectionChanged += HandleSelectionChanged;
        _inventoryService.OnItemRemoved += HandleItemRemoved;

        if (_loadFromPlayerPrefsOnAwake)
        {
            LoadFromPlayerPrefs();
        }
    }

    #region Managing Inventory
    public void ClearSelection()
    {
        _inventoryService.ClearSelection();
    }

    public void ToggleInventory()
    {
        _isInventoryUIOn = !_isInventoryUIOn;
        OnInventoryToggled?.Invoke(_isInventoryUIOn);
    }

    public void SelectItem(int index)
    {
        _inventoryService.Select(index);
    }

    public ItemInstanceData CreateItemInstance(int itemId)
    {
        ItemInstanceData itemInstanceData = _instanceInstanceService.CreateInstance(itemId);
        if (itemInstanceData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Failed to create ItemInstance for item id {itemId}.", this);
        }

        return itemInstanceData;
    }

    public bool TryAddItem(ItemInstanceData itemInstanceData)
    {
        if (itemInstanceData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Tried to add a null ItemInstance to inventory.", this);
            return false;
        }

        _instanceInstanceService.RegisterInstance(itemInstanceData);
        return _inventoryService.TryAdd(itemInstanceData.InstanceId);
    }

    public void RemoveItem(int index)
    {
        _inventoryService.RemoveAt(index);
    }

    public bool RemoveItem(ItemInstanceData itemInstanceData)
    {
        if (itemInstanceData == null)
        {
            return false;
        }

        return _inventoryService.Remove(itemInstanceData.InstanceId);
    }

    public bool RemoveCurrentHandItem()
    {
        if (string.IsNullOrEmpty(_handService.EquippedInstanceId))
        {
            return false;
        }

        return _inventoryService.Remove(_handService.EquippedInstanceId);
    }

    public bool HasItem(int itemId)
    {
        foreach (string instanceId in _inventoryService.Items)
        {
            ItemInstanceData item = GetItemInstance(instanceId);
            if (item != null && item.ItemId == itemId)
            {
                return true;
            }
        }

        return false;
    }

    public int IndexOf(ItemInstanceData itemInstanceData)
    {
        return itemInstanceData == null ? -1 : _inventoryService.IndexOf(itemInstanceData.InstanceId);
    }

    public int IndexOf(string instanceId)
    {
        return _inventoryService.IndexOf(instanceId);
    }

    public ItemInstanceData GetItemInstance(string instanceId)
    {
        return _instanceInstanceService.GetInstance(instanceId);
    }

    public bool TryGetItemInstance(string instanceId, out ItemInstanceData itemInstanceData)
    {
        return _instanceInstanceService.TryGetInstance(instanceId, out itemInstanceData);
    }

    public string GetInventoryItemInstanceIdAt(int index)
    {
        if (index < 0 || index >= _inventoryService.Count)
        {
            return null;
        }

        return _inventoryService.GetAt(index);
    }

    public void SwapItem(int index1, int index2)
    {
        _inventoryService.Swap(index1, index2);
    }
    #endregion

    #region Examine
    public GameObject ShowExamineItem(ItemInstanceData itemInstanceData)
    {
        return ShowExamineItem(itemInstanceData != null ? itemInstanceData.InstanceId : null);
    }

    public GameObject ShowExamineItem(string instanceId)
    {
        ItemInstanceData itemInstanceData = GetItemInstance(instanceId);
        if (itemInstanceData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Cannot show examine item because ItemInstance is null.", this);
            return null;
        }

        return _examineService.Show(instanceId);
    }

    public void HideExamineItem()
    {
        _examineService.Hide();
    }
    #endregion

    #region World Object
    public GameObject CreateWorldItem(ItemInstanceData itemInstanceData, Transform itemRoot)
    {
        return CreateWorldItem(itemInstanceData != null ? itemInstanceData.InstanceId : null, itemRoot);
    }

    public GameObject CreateWorldItem(string instanceId, Transform itemRoot)
    {
        ItemInstanceData itemInstanceData = GetItemInstance(instanceId);
        if (itemInstanceData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Cannot create a world item from a null ItemInstance.", this);
            return null;
        }

        return _worldService.Create(instanceId, itemRoot);
    }
    #endregion

    #region Hand Object
    public GameObject ShowHandItem(ItemInstanceData itemInstanceData)
    {
        return ShowHandItem(itemInstanceData != null ? itemInstanceData.InstanceId : null);
    }

    public GameObject ShowHandItem(string instanceId)
    {
        ItemInstanceData itemInstanceData = GetItemInstance(instanceId);
        if (itemInstanceData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Cannot show hand item because ItemInstance is null.", this);
            return null;
        }

        return _handService.Show(instanceId);
    }

    public void HideHandItem()
    {
        _handService.Hide();
    }
    #endregion

    #region Save
    public InventorySaveData CaptureSaveData()
    {
        SyncRegisteredSceneStates();

        InventorySaveData saveData = new InventorySaveData
        {
            EquippedHandInstanceId = _handService.EquippedInstanceId,
            SelectedIndex = _inventoryService.SelectedIndex
        };

        foreach (ItemInstanceData itemInstanceData in _instanceInstanceService.GetAllInstances())
        {
            if (itemInstanceData == null)
            {
                continue;
            }

            saveData.ItemInstances.Add(new ItemInstanceSaveData
            {
                InstanceId = itemInstanceData.InstanceId,
                ItemId = itemInstanceData.ItemId,
                StateEntries = itemInstanceData.State != null ? itemInstanceData.State.CaptureSaveData() : new List<ItemStateValueSaveData>()
            });
        }

        foreach (string instanceId in _inventoryService.Items)
        {
            saveData.InventoryInstanceIds.Add(instanceId);
        }

        foreach (SceneItemSaveData sceneItemState in _sceneItemStates.Values)
        {
            saveData.SceneItems.Add(new SceneItemSaveData
            {
                SceneObjectId = sceneItemState.SceneObjectId,
                InstanceId = sceneItemState.InstanceId,
                IsCollected = sceneItemState.IsCollected
            });
        }

        return saveData;
    }

    public string CaptureSaveJson(bool prettyPrint = false)
    {
        return JsonUtility.ToJson(CaptureSaveData(), prettyPrint);
    }

    public void RestoreSaveData(InventorySaveData saveData)
    {
        _handService.ResetState();
        _examineService.ResetState();
        _instanceInstanceService.Clear();
        _sceneItemStates.Clear();

        if (saveData != null)
        {
            if (saveData.ItemInstances != null)
            {
                foreach (ItemInstanceSaveData itemSaveData in saveData.ItemInstances)
                {
                    _instanceInstanceService.RestoreInstance(itemSaveData);
                }
            }

            if (saveData.SceneItems != null)
            {
                foreach (SceneItemSaveData sceneItem in saveData.SceneItems)
                {
                    if (sceneItem == null || string.IsNullOrEmpty(sceneItem.SceneObjectId))
                    {
                        continue;
                    }

                    _sceneItemStates[sceneItem.SceneObjectId] = new SceneItemSaveData
                    {
                        SceneObjectId = sceneItem.SceneObjectId,
                        InstanceId = sceneItem.InstanceId,
                        IsCollected = sceneItem.IsCollected
                    };
                }
            }
        }

        _inventoryService.Restore(
            saveData != null ? saveData.InventoryInstanceIds : null,
            saveData != null ? saveData.SelectedIndex : -1);

        RebindRegisteredSceneItems();

        string equippedInstanceId = saveData != null ? saveData.EquippedHandInstanceId : null;
        if (!string.IsNullOrEmpty(equippedInstanceId) && GetItemInstance(equippedInstanceId) != null)
        {
            _handService.Show(equippedInstanceId);
        }
        else
        {
            _handService.Hide();
        }
    }

    public void RestoreSaveJson(string saveJson)
    {
        if (string.IsNullOrEmpty(saveJson))
        {
            RestoreSaveData(null);
            return;
        }

        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(saveJson);
        RestoreSaveData(saveData);
    }

    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetString(ResolveSaveKey(), CaptureSaveJson());
        PlayerPrefs.Save();
    }

    public bool LoadFromPlayerPrefs()
    {
        string saveKey = ResolveSaveKey();
        if (!PlayerPrefs.HasKey(saveKey))
        {
            return false;
        }

        RestoreSaveJson(PlayerPrefs.GetString(saveKey));
        return true;
    }

    public void DeletePlayerPrefsSave()
    {
        PlayerPrefs.DeleteKey(ResolveSaveKey());
        PlayerPrefs.Save();
    }
    #endregion

    #region Scene Binding
    public void RegisterSceneItem(PersistentSceneItem sceneItem)
    {
        if (sceneItem == null || string.IsNullOrEmpty(sceneItem.SceneObjectId))
        {
            return;
        }

        _registeredSceneItems[sceneItem.SceneObjectId] = sceneItem;
        ApplySceneItemState(sceneItem);
    }

    public void UnregisterSceneItem(PersistentSceneItem sceneItem)
    {
        if (sceneItem == null || string.IsNullOrEmpty(sceneItem.SceneObjectId))
        {
            return;
        }

        if (_registeredSceneItems.TryGetValue(sceneItem.SceneObjectId, out PersistentSceneItem current) && current == sceneItem)
        {
            _registeredSceneItems.Remove(sceneItem.SceneObjectId);
        }
    }

    public void MarkSceneItemCollected(string sceneObjectId, string instanceId)
    {
        if (string.IsNullOrEmpty(sceneObjectId))
        {
            return;
        }

        _sceneItemStates[sceneObjectId] = new SceneItemSaveData
        {
            SceneObjectId = sceneObjectId,
            InstanceId = instanceId,
            IsCollected = true
        };
    }
    #endregion

    #region Binding Helpers
    private void HandleInventoryChanged()
    {
        OnDataChanged?.Invoke();
    }

    private void HandleSelectionChanged(int selectedIndex)
    {
        _selectedIndex = selectedIndex;
        OnSelectionChanged?.Invoke(selectedIndex);
    }

    private void HandleItemRemoved(string instanceId)
    {
        _examineService.Remove(instanceId);
        _handService.Remove(instanceId);
    }

    private Transform ResolveRoot(Transform root)
    {
        return root != null ? root : transform;
    }

    private string ResolveSaveKey()
    {
        return string.IsNullOrEmpty(_playerPrefsSaveKey) ? DefaultSaveKey : _playerPrefsSaveKey;
    }

    private void RebindRegisteredSceneItems()
    {
        foreach (PersistentSceneItem sceneItem in _registeredSceneItems.Values)
        {
            ApplySceneItemState(sceneItem);
        }
    }

    private void SyncRegisteredSceneStates()
    {
        foreach (PersistentSceneItem sceneItem in _registeredSceneItems.Values)
        {
            if (sceneItem == null || string.IsNullOrEmpty(sceneItem.SceneObjectId))
            {
                continue;
            }

            string instanceId = sceneItem.InstanceView != null ? sceneItem.InstanceView.InstanceId : null;
            bool isCollected = _sceneItemStates.TryGetValue(sceneItem.SceneObjectId, out SceneItemSaveData state) && state.IsCollected;

            _sceneItemStates[sceneItem.SceneObjectId] = new SceneItemSaveData
            {
                SceneObjectId = sceneItem.SceneObjectId,
                InstanceId = string.IsNullOrEmpty(instanceId) && state != null ? state.InstanceId : instanceId,
                IsCollected = isCollected
            };
        }
    }

    private void ApplySceneItemState(PersistentSceneItem sceneItem)
    {
        if (sceneItem == null || string.IsNullOrEmpty(sceneItem.SceneObjectId))
        {
            return;
        }

        SceneItemSaveData sceneState = GetOrCreateSceneState(sceneItem);
        if (sceneState.IsCollected)
        {
            sceneItem.ApplyCollectedState(true);
            return;
        }

        sceneItem.ApplyCollectedState(false);

        if (sceneItem.InstanceView == null)
        {
            return;
        }

        string instanceId = sceneState.InstanceId;
        if (string.IsNullOrEmpty(instanceId) || GetItemInstance(instanceId) == null)
        {
            ItemInstanceData itemInstanceData = CreateItemInstance(sceneItem.InstanceView.InitialItemKey);
            if (itemInstanceData == null)
            {
                return;
            }

            instanceId = itemInstanceData.InstanceId;
            sceneState.InstanceId = instanceId;
        }

        sceneItem.InstanceView.Bind(instanceId, this);
    }

    private SceneItemSaveData GetOrCreateSceneState(PersistentSceneItem sceneItem)
    {
        if (_sceneItemStates.TryGetValue(sceneItem.SceneObjectId, out SceneItemSaveData existing))
        {
            return existing;
        }

        SceneItemSaveData created = new SceneItemSaveData
        {
            SceneObjectId = sceneItem.SceneObjectId,
            InstanceId = sceneItem.InstanceView != null ? sceneItem.InstanceView.InstanceId : null,
            IsCollected = false
        };

        _sceneItemStates[sceneItem.SceneObjectId] = created;
        return created;
    }
    #endregion
}
