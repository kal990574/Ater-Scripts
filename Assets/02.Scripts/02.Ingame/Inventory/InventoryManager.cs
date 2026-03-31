using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    public static InventoryManager Instance => _instance;
    
    [Header("Reference")]
    [SerializeField] private ItemDataTableSO tableSo;
    [SerializeField] private Transform _cachedExamineRoot;
    [SerializeField] private Transform _cachedHandRoot;
    
    [Header("Debug/DontChange")]
    [SerializeField] private bool _isInventoryUIOn = false;
    [SerializeField] private int _selectedIndex = -1; //UI전용
    
    //인벤토리 서비스 
    private InventoryService _inventoryService;

    //생성된 인스턴스 생성 및 관리
    private RuntimeInstanceService _runtimeInstanceService;
    
    //들고 있는 아이템에 대한 서비스
    private HandViewService handViewService;
    
    //조사하기에 대한 서비스
    private ExamineViewService examineViewService;
    
    //월드 오브젝트에 대한 서비스
    private WorldViewService worldViewService;
    
    //각 아이템을 생성하는 팩토리
    private ItemFactory _itemFactory;
    
    public IReadOnlyList<string> ReadonlyPlayerInventoryInstanceIds => _inventoryService.Items;
    public int Count => _inventoryService.Count;
    public int SelectedIndex => _inventoryService.SelectedIndex;
    public string CurrentHandItemInstanceId => handViewService.EquippedInstanceId;
    public RuntimeItemData CurrentHand => GetItemInstance(handViewService.EquippedInstanceId);
    
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

        _runtimeInstanceService = new RuntimeInstanceService(tableSo);
        _runtimeInstanceService.SetItemDataTable(tableSo);
        _inventoryService = new InventoryService();
        _itemFactory = new ItemFactory(_runtimeInstanceService, this);
        handViewService = new HandViewService(_itemFactory, ResolveRoot(_cachedHandRoot));
        examineViewService = new ExamineViewService(_itemFactory, ResolveRoot(_cachedExamineRoot));
        worldViewService = new WorldViewService(_itemFactory);
        
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
    public RuntimeItemData CreateItemInstance(int itemId)
    {
        RuntimeItemData runtimeItemData = _runtimeInstanceService.CreateInstance(itemId);
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Failed to create ItemInstance for item id {itemId}.", this);
        }

        return runtimeItemData;
    }

    public RuntimeData CreateRuntimeData(InteractState defaultState = null)
    {
        RuntimeData runtimeData = _runtimeInstanceService.CreateRuntimeData(defaultState);
        if (runtimeData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Failed to create runtime data.", this);
        }

        return runtimeData;
    }

    public RuntimeData GetOrCreateRuntimeData(string instanceId, InteractState defaultState = null)
    {
        RuntimeData runtimeData = _runtimeInstanceService.GetOrCreateRuntimeData(instanceId, defaultState);
        if (runtimeData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Failed to get or create runtime data for instance id '{instanceId}'.", this);
        }

        return runtimeData;
    }

    public RuntimeItemData GetOrCreateItemInstance(string instanceId, int itemId)
    {
        RuntimeItemData runtimeItemData = _runtimeInstanceService.GetOrCreateItemInstance(instanceId, itemId);
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Failed to get or create item instance '{instanceId}' for item id {itemId}.", this);
        }

        return runtimeItemData;
    }

    //아이템 인스턴스로 인벤토리에 아이템 추가
    public bool TryAddItem(RuntimeItemData runtimeItemData)
    {
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Tried to add a null ItemInstance to inventory.", this);
            return false;
        }

        _runtimeInstanceService.RegisterInstance(runtimeItemData);
        return _inventoryService.TryAdd(runtimeItemData.InstanceId);
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
        if (string.IsNullOrEmpty(handViewService.EquippedInstanceId))
        {
            return false;
        }

        return _inventoryService.Remove(handViewService.EquippedInstanceId);
    }

    //해당 인스턴스 아이디를 가진 아이템이 몇번째 칸에 있는지 확인
    public int IndexOf(string instanceId)
    {
        return _inventoryService.IndexOf(instanceId);
    }

    //인스턴스 아이디로 데이터 탐색후 반환
    public RuntimeItemData GetItemInstance(string instanceId)
    {
        return _runtimeInstanceService.GetItemInstance(instanceId);
    }

    public RuntimeData GetRuntimeData(string instanceId)
    {
        return _runtimeInstanceService.GetRuntimeData(instanceId);
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
        RuntimeItemData runtimeItemData = GetItemInstance(instanceId);
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Cannot show examine item because ItemInstance is null.", this);
            return null;
        }

        return examineViewService.Show(instanceId);
    }

    public void HideExamineItem()
    {
        examineViewService.Hide();
    }
    #endregion

    #region World Object
    public GameObject CreateWorldItem(string instanceId, Transform itemRoot)
    {
        RuntimeItemData runtimeItemData = GetItemInstance(instanceId);
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Cannot create a world item from a null ItemInstance.", this);
            return null;
        }

        return worldViewService.Create(instanceId, itemRoot);
    }
    #endregion
    
    #region Hand Object
    public GameObject ShowHandItem(string instanceId)
    {
        RuntimeItemData runtimeItemData = GetItemInstance(instanceId);
        if (runtimeItemData == null)
        {
            Debug.LogError($"[{nameof(InventoryManager)}] Cannot show hand item because ItemInstance is null.", this);
            return null;
        }
        return handViewService.Show(instanceId);
    }

    public void HideHandItem()
    {
        handViewService.Hide();
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
        examineViewService.Remove(instanceId);
        handViewService.Remove(instanceId);
    }

    private Transform ResolveRoot(Transform root)
    {
        return root != null ? root : transform;
    }
    #endregion
}
