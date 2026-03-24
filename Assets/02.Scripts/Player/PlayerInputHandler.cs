using UnityEngine;
using UnityEngine.InputSystem;

namespace _02.Scripts.Player
{
    public class PlayerInputHandler : MonoBehaviour, IPlayerInput
    {
        [SerializeField] private InputActionAsset _inputActions;
        
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _lmbAction;
        private InputAction _rmbAction;
        private InputAction _interactAction;
        private InputAction _scannerToggleAction;
        private InputAction _inventoryToggleAction;
        private InputAction _hintToggleAction;
        private InputAction[] _itemSlotActions;
        
        public Vector2 MoveInput => _moveAction.ReadValue<Vector2>();
        public Vector2 LookInput => _lookAction.ReadValue<Vector2>();
        public bool LmbPressInput => _lmbAction.IsPressed();
        public bool LmbReleaseInput => _lmbAction.WasReleasedThisFrame();
        public bool RmbPressInput  => _rmbAction.WasPressedThisFrame();
        public bool RmbReleaseInput => _rmbAction.WasReleasedThisFrame();
        public bool InteractInput  => _interactAction.WasPressedThisFrame();
        public bool ScannerToggleInput => _scannerToggleAction.WasPressedThisFrame();
        public bool InventoryToggleInput => _inventoryToggleAction.WasPressedThisFrame();
        public bool HintToggleInput => _hintToggleAction.WasPressedThisFrame();

        public int ItemSlotInput
        {
            get
            {
                for (int i = 0; i < _itemSlotActions.Length; i++)
                {
                    if (_itemSlotActions[i].WasPressedThisFrame())
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        private void Awake()
        {
            InputActionMap playerMap = _inputActions.FindActionMap("Player");
            _moveAction = playerMap.FindAction("Move");
            _lookAction = playerMap.FindAction("Look");
            _lmbAction = playerMap.FindAction("LMB");
            _rmbAction = playerMap.FindAction("RMB");
            _interactAction = playerMap.FindAction("Interact");
            _scannerToggleAction = playerMap.FindAction("ScannerToggle");
            _inventoryToggleAction = playerMap.FindAction("InventoryToggle");
            _hintToggleAction = playerMap.FindAction("HintToggle");
            _itemSlotActions = new InputAction[5];
            for (int i = 0; i < 5; i++)
            {
                _itemSlotActions[i] = playerMap.FindAction($"ItemSlot{i + 1}");
            }
        }

        private void OnEnable()
        {
            _inputActions.FindActionMap("Player").Enable();
        }

        private void OnDisable()
        {
            _inputActions.FindActionMap("Player").Disable();
        }
    }
}