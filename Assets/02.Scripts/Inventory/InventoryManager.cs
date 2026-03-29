using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    public static InventoryManager Instance => _instance;
    
    [Header("Reference")]
    [SerializeField] private ItemDataTable _table;
    [SerializeField] private Transform _cachedExamineRoot;
    [SerializeField] private Transform _cachedHandRoot;
    
    [Header("Debug/DontChange")]
    [SerializeField] private bool _isInventoryUIOn = false;
    [SerializeField] private int _selectedIndex = -1;
    
    //인벤토리 서비스 
    private InventoryService _inventoryService;
    //생성된 인스턴스 생성 및 관리
    private RuntimeInstanceService _instanceInstanceService;
    
    //들고 있는 아이템에 대한 서비스
    private HandService _handService;
    
    //조사하기에 대한 서비스
    private ExamineService _examineService;
    
    //월드 오브젝트에 대한 서비스
    private WorldService _worldService;
    
    //각 아이템을 생성하는 팩토리
    private ItemFactory _itemFactory;
    
    public IReadOnlyList<string> ReadonlyPlayerInventoryInstanceIds => _inventoryService.Items;
    public int Count => _inventoryService.Count;
    public int SelectedIndex => _inventoryService.SelectedIndex;
    public string CurrentHandItemInstanceId => _handService.EquippedInstanceId;
    public ItemInstanceData CurrentHandItem => GetItemInstance(_handService.EquippedInstanceId);
    
    public event Action<bool> OnInventoryToggled;
    public event Action OnInventoryItemChanged;
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
    
    //아이템 아이디로 새로운 인스턴스 제작
    public ItemInstanceData CreateItemInstance(int itemId)
    {
        ItemInstanceData itemInstanceData = _instanceInstanceService.CreateInstance(itemId);
        if (itemInstanceData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Failed to create ItemInstance for item id {itemId}.", this);
        }

        return itemInstanceData;
    }

    //아이템 인스턴스로 인벤토리에 아이템 추가
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

    //인벤토리의 해당칸에 위치한 아이템 제거
    public void RemoveItem(int index)
    {
        _inventoryService.RemoveAt(index);
    }
    
    //아이템의 위치 변경
    public void SwapItem(int index1, int index2)
    {
        _inventoryService.Swap(index1, index2);
    }
    
    //현재 손에든 아이템을 제거함
    public bool RemoveCurrentHandItem()
    {
        if (string.IsNullOrEmpty(_handService.EquippedInstanceId))
        {
            return false;
        }

        return _inventoryService.Remove(_handService.EquippedInstanceId);
    }

    //해당 인스턴스 아이디를 가진 아이템이 몇번째 칸에 있는지 확인
    public int IndexOf(string instanceId)
    {
        return _inventoryService.IndexOf(instanceId);
    }

    //인스턴스 아이디로 데이터 탐색후 반환
    public ItemInstanceData GetItemInstance(string instanceId)
    {
        return _instanceInstanceService.GetInstance(instanceId);
    }

    public string GetInventoryItemInstanceIdAt(int index)
    {
        if (index < 0 || index >= _inventoryService.Count)
        {
            return null;
        }

        return _inventoryService.GetAt(index);
    }


    #endregion
    
    #region Examine
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

    #region Binding Helpers
    private void HandleInventoryChanged()
    {
        OnInventoryItemChanged?.Invoke();
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
    #endregion
}
