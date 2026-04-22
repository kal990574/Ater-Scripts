using UnityEngine;

namespace _02.Scripts.Player
{
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
        private RuntimeItemFactory _itemFactory;
        private string _currentHandInstanceId;
        private bool _isChargingThrow;
        private float _throwChargeTime;

        public int CurrentHandIndex => _handIndex;
        public string CurrentHandItemInstanceId => _currentHandInstanceId;
        public bool HasHandItem => _handIndex >= 0 && string.IsNullOrEmpty(_currentHandInstanceId) == false;
        public int InventoryCount => _inventoryManager != null ? _inventoryManager.Count : 0;

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
            _handViewService = new HandViewService(_itemFactory, _inventoryManager, _runtimeInstanceManager, _handRoot);
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

        public bool CanPickUpItem(int num)
        {
            return TryGetPickableItemInstanceId(num, out _);
        }

        public bool TryPickUpItem(int num)
        {
            if (_handViewService == null)
            {
                return false;
            }

            if (TryGetPickableItemInstanceId(num, out string instanceId) == false)
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
        
        public bool TryConsumeCurrentHandItem(out HandConsumeResult result)
        {
            if (_inventoryManager == null || HasHandItem == false)
            {
                result = new HandConsumeResult(false, false, -1);
                return false;
            }

            bool isRemoved = _inventoryManager.RemoveItem(_handIndex);
            if (isRemoved == false)
            {
                result = new HandConsumeResult(false, false, -1);
                return false;
            }

            ClearHandItem();
            bool inventoryEmptyAfterConsume = _inventoryManager.Count <= 0;
            result = new HandConsumeResult(true, inventoryEmptyAfterConsume, -1);
            return true;
        }

        public void ClearHandItem()
        {
            if (_handIndex < 0 && string.IsNullOrEmpty(_currentHandInstanceId))
            {
                return;
            }

            _handIndex = -1;
            _currentHandInstanceId = null;
            _handViewService?.Hide();
            NotifyHandSlotChanged();
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
                    return;
                }

                int resolvedIndex = ResolveNextHandIndex(_handIndex);
                if (resolvedIndex < 0)
                {
                    ClearHandItem();
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

        private bool TryGetPickableItemInstanceId(int num, out string instanceId)
        {
            instanceId = null;

            if (_inventoryManager == null)
            {
                return false;
            }

            if (num < 0 || num >= _inventoryManager.Count)
            {
                return false;
            }

            if (_inventoryManager.TryGetInventoryItemInstanceIdAt(num, out instanceId) == false)
            {
                return false;
            }

            return string.IsNullOrEmpty(instanceId) == false;
        }

        private void NotifyHandSlotChanged()
        {
            OnHandSlotChanged?.Invoke(_handIndex);
        }
    }
}
