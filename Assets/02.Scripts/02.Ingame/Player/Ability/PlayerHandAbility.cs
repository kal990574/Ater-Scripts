using _02.Scripts.Player;
using UnityEngine;

public class PlayerHandAbility : PlayerAbility
{
    [SerializeField] private Transform _handRoot;
    [SerializeField] private float _throwDistance = 1.5f;
    [SerializeField] private float _throwHoldThreshold = 0.2f;
    [SerializeField] private float _maxThrowChargeTime = 1.5f;
    [SerializeField] private float _maxThrowForce = 10.0f;
    [SerializeField] private int _handIndex = -1;

    private InventoryManager _inventoryManager;
    private RuntimeInstanceManager _runtimeInstanceManager;
    private HandViewService _handViewService;
    private WorldViewService _worldViewService;
    private RuntimeItemFactory _itemFactory;
    private string _currentHandInstanceId;
    private bool _isChargingThrow;
    private float _throwChargeTime;

    public int CurrentHandIndex => _handIndex;
    public string CurrentHandItemInstanceId => _currentHandInstanceId;
    public bool HasHandItem => _handIndex >= 0 && string.IsNullOrEmpty(_currentHandInstanceId) == false;

    public event System.Action<int> OnHandSlotChanged;
    
    private void Start()
    {
        if (_inventoryManager == null)
        {
            _inventoryManager = InventoryManager.Instance;
        }

        if (_runtimeInstanceManager == null)
        {
            _runtimeInstanceManager = RuntimeInstanceManager.Instance;
        }

        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryItemChanged += SyncCurrentHandItemState;
        }

        _itemFactory = new RuntimeItemFactory(_runtimeInstanceManager);
        _handViewService = new HandViewService(_itemFactory, _inventoryManager,  _runtimeInstanceManager, _handRoot);
        _worldViewService = new WorldViewService(_itemFactory);
    }

    private void OnDestroy()
    {
        if (_inventoryManager != null)
        {
            _inventoryManager.OnInventoryItemChanged -= SyncCurrentHandItemState;
        }
    }

    public void ToggleInventory()
    {
        if (_inventoryManager == null)
        {
            return;
        }

        _inventoryManager.ToggleInventory();
    }

    public bool TryPickUpItem(int num)
    {
        if (_inventoryManager == null || _handViewService == null)
        {
            return false;
        }

        if (num < 0 || num >= _inventoryManager.Count)
        {
            return false;
        }

        if (_inventoryManager.TryGetInventoryItemInstanceIdAt(num, out string instanceId) == false)
        {
            return false;
        }

        GameObject handObject = _handViewService.Show(num);
        if (handObject == null)
        {
            return false;
        }

        _handIndex = num;
        _currentHandInstanceId = instanceId;
        NotifyHandSlotChanged();
        return true;
    }

    public void BeginReleaseHandItem()
    {
        if (_inventoryManager == null || string.IsNullOrEmpty(_currentHandInstanceId))
        {
            return;
        }

        _isChargingThrow = true;
        _throwChargeTime = 0.0f;
    }

    public void ChargeReleaseHandItem(float deltaTime)
    {
        if (_isChargingThrow == false)
        {
            return;
        }

        _throwChargeTime += deltaTime;
    }

    public bool ReleaseHandItem()
    {
        if (_isChargingThrow == false)
        {
            return false;
        }

        if (_inventoryManager == null || _runtimeInstanceManager == null || _worldViewService == null)
        {
            ResetThrowCharge();
            return false;
        }

        RuntimeItemData runtimeItemData = _runtimeInstanceManager.GetItemInstance(_currentHandInstanceId);
        if (runtimeItemData == null)
        {
            ResetThrowCharge();
            return false;
        }

        bool shouldThrow = _throwChargeTime >= _throwHoldThreshold;
        GameObject worldObject = _worldViewService.Create(runtimeItemData, null);
        if (worldObject == null)
        {
            ResetThrowCharge();
            return false;
        }

        worldObject.transform.position = _owner.transform.position + (_owner.transform.forward * _throwDistance);
        worldObject.transform.rotation = Quaternion.identity;

        if (shouldThrow)
        {
            ApplyThrowForce(worldObject);
        }

        int nextHandIndex = GetNextHandIndexAfterThrow();
        bool isRemoved = _inventoryManager.RemoveItem(_handIndex);
        if (isRemoved == false)
        {
            Object.Destroy(worldObject);
            ResetThrowCharge();
            return false;
        }

        ClearHandItem();
        ResetThrowCharge();

        if (_inventoryManager.Count <= 0)
        {
            SwitchToScanMode();
            return true;
        }

        int resolvedIndex = ResolveNextHandIndex(nextHandIndex);
        if (resolvedIndex < 0)
        {
            SwitchToScanMode();
            return true;
        }

        TryPickUpItem(resolvedIndex);
        return true;
    }

    public bool TryConsumeCurrentHandItem()
    {
        if (_inventoryManager == null || HasHandItem == false)
        {
            return false;
        }

        int nextHandIndex = GetNextHandIndexAfterThrow();
        bool isRemoved = _inventoryManager.RemoveItem(_handIndex);
        if (isRemoved == false)
        {
            return false;
        }

        ClearHandItem();

        if (_inventoryManager.Count <= 0)
        {
            SwitchToScanMode();
            return true;
        }

        int resolvedIndex = ResolveNextHandIndex(nextHandIndex);
        if (resolvedIndex < 0)
        {
            SwitchToScanMode();
            return true;
        }

        return TryPickUpItem(resolvedIndex);
    }

    public void ClearHandItem()
    {
        _handIndex = -1;
        _currentHandInstanceId = null;
        ResetThrowCharge();
        _handViewService?.Hide();
        NotifyHandSlotChanged();
    }

    public void CycleHandItem(int direction)
    {
        if (_inventoryManager == null || _inventoryManager.Count == 0)
        {
            return;
        }

        int current = _handIndex < 0 ? 0 : _handIndex;
        int newIndex = (current + direction + _inventoryManager.Count) % _inventoryManager.Count;
        TryPickUpItem(newIndex);
    }

    private void SyncCurrentHandItemState()
    {
        if (_inventoryManager == null || string.IsNullOrEmpty(_currentHandInstanceId))
        {
            return;
        }

        int currentIndex = _inventoryManager.IndexOf(_currentHandInstanceId);
        if (currentIndex < 0)
        {
            if (_inventoryManager.Count <= 0)
            {
                ClearHandItem();
                SwitchToScanMode();
                return;
            }

            int resolvedIndex = ResolveNextHandIndex(_handIndex);
            if (resolvedIndex < 0)
            {
                ClearHandItem();
                SwitchToScanMode();
                return;
            }

            TryPickUpItem(resolvedIndex);
            return;
        }

        if (_handIndex != currentIndex)
        {
            _handIndex = currentIndex;
            NotifyHandSlotChanged();
        }
    }

    private int GetNextHandIndexAfterThrow()
    {
        int itemCountAfterRemoval = _inventoryManager.Count - 1;
        if (itemCountAfterRemoval <= 0)
        {
            return -1;
        }

        if (_handIndex < itemCountAfterRemoval)
        {
            return _handIndex;
        }

        return itemCountAfterRemoval - 1;
    }

    private int ResolveNextHandIndex(int preferredIndex)
    {
        if (_inventoryManager == null || _inventoryManager.Count <= 0)
        {
            return -1;
        }

        if (preferredIndex < 0)
        {
            return -1;
        }

        if (preferredIndex < _inventoryManager.Count)
        {
            return preferredIndex;
        }

        return _inventoryManager.Count - 1;
    }

    private void ApplyThrowForce(GameObject worldObject)
    {
        Rigidbody rigidbody = worldObject.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = worldObject.GetComponentInChildren<Rigidbody>();
        }

        if (rigidbody == null)
        {
            return;
        }

        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        float chargeRatio = _maxThrowChargeTime <= 0.0f
            ? 1.0f
            : Mathf.Clamp01(_throwChargeTime / _maxThrowChargeTime);
        float throwForce = _maxThrowForce * chargeRatio;

        rigidbody.AddForce(_owner.transform.forward * throwForce, ForceMode.Impulse);
    }

    private void ResetThrowCharge()
    {
        _isChargingThrow = false;
        _throwChargeTime = 0.0f;
    }

    private void SwitchToScanMode()
    {
        if (_owner == null)
        {
            return;
        }

        _owner.SetInteractMode(EPlayerInteractMode.Scan);
    }

    private void NotifyHandSlotChanged()
    {
        OnHandSlotChanged?.Invoke(_handIndex);
    }
}