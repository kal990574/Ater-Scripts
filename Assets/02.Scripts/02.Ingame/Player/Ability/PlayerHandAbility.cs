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

        public bool TryConsumeCurrentHandItem()
        {
            return TryConsumeCurrentHandItem(out _);
        }

        public bool TryConsumeCurrentHandItem(out HandConsumeResult result)
        {
            if (_inventoryManager == null || HasHandItem == false)
            {
                result = new HandConsumeResult(false, false, -1);
                return false;
            }

            int nextHandIndex = GetNextHandIndexAfterThrow();
            bool isRemoved = _inventoryManager.RemoveItem(_handIndex);
            if (isRemoved == false)
            {
                result = new HandConsumeResult(false, false, -1);
                return false;
            }

            ClearHandItem();

            if (_inventoryManager.Count <= 0)
            {
                result = new HandConsumeResult(true, true, -1);
                return true;
            }

            int resolvedIndex = ResolveNextHandIndex(nextHandIndex);
            if (resolvedIndex < 0)
            {
                result = new HandConsumeResult(true, false, -1);
                return true;
            }

            bool didEquipNext = TryPickUpItem(resolvedIndex);
            result = new HandConsumeResult(true, false, didEquipNext ? resolvedIndex : -1);
            return true;
        }

        public void ClearHandItem()
        {
            _handIndex = -1;
            _currentHandInstanceId = null;
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

        private void NotifyHandSlotChanged()
        {
            OnHandSlotChanged?.Invoke(_handIndex);
        }
    }
}
