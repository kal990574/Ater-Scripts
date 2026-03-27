using _02.Scripts.Player;
using UnityEngine;

public class PlayerInventoryAbility : PlayerAbility
{
    [SerializeField] private float _throwDistance = 1.5f;
    [SerializeField] private float _throwHoldThreshold = 0.2f;
    [SerializeField] private float _maxThrowChargeTime = 1.5f;
    [SerializeField] private float _maxThrowForce = 10.0f;
    [SerializeField] private int _handIndex = -1;
    
    private InventoryManager _inventoryManager;
    private string _currentHandInstanceId;
    private bool _isChargingThrow;
    private float _throwChargeTime;
    
    private void Start()
    {
        _inventoryManager = InventoryManager.Instance;
        if (_inventoryManager != null)
        {
            _inventoryManager.OnDataChanged += SyncCurrentHandItemState;
        }
    }

    private void OnDestroy()
    {
        if (_inventoryManager != null)
        {
            _inventoryManager.OnDataChanged -= SyncCurrentHandItemState;
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
        if (_inventoryManager == null)
        {
            return false;
        }

        if (num < 0 || num >= _inventoryManager.Count)
        {
            return false;
        }

        string instanceId = _inventoryManager.GetInventoryItemInstanceIdAt(num);
        if (string.IsNullOrEmpty(instanceId))
        {
            return false;
        }
        
        _inventoryManager.HideHandItem();
        _handIndex = num;
        _currentHandInstanceId = instanceId;

        return _inventoryManager.ShowHandItem(instanceId) != null;
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
        if (_isChargingThrow == false || _inventoryManager == null || string.IsNullOrEmpty(_currentHandInstanceId))
        {
            return false;
        }

        bool shouldThrow = _throwChargeTime >= _throwHoldThreshold;
        GameObject worldObject = _inventoryManager.CreateWorldItem(_currentHandInstanceId, null);
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
        _inventoryManager.RemoveItem(_handIndex);
        ClearHandItem();
        ResetThrowCharge();
        TryPickUpItem(nextHandIndex);
        return true;
    }
    
    public void ClearHandItem()
    {
        _handIndex = -1;
        _currentHandInstanceId = null;
        ResetThrowCharge();
        _inventoryManager?.HideHandItem();
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
            _handIndex = -1;
            _currentHandInstanceId = null;
            _inventoryManager.HideHandItem();
            return;
        }

        _handIndex = currentIndex;
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

        float chargeRatio = _maxThrowChargeTime <= 0.0f ? 1.0f : Mathf.Clamp01(_throwChargeTime / _maxThrowChargeTime);
        float throwForce = _maxThrowForce * chargeRatio;

        rigidbody.AddForce(_owner.transform.forward * throwForce, ForceMode.Impulse);
    }

    private void ResetThrowCharge()
    {
        _isChargingThrow = false;
        _throwChargeTime = 0.0f;
    }
}
