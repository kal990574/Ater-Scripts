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
        public Vector2 MoveInput => _moveAction.ReadValue<Vector2>();
        public Vector2 LookInput => _lookAction.ReadValue<Vector2>();
        public bool LmbPressInput => _lmbAction.IsPressed();
        public bool LmbReleaseInput => _lmbAction.WasReleasedThisFrame();
        public bool RmbPressInput  => _rmbAction.WasPressedThisFrame();
        public bool RmbReleaseInput => _rmbAction.WasReleasedThisFrame();
        public bool InteractInput  => _interactAction.WasPressedThisFrame();

        private void Awake()
        {
            InputActionMap playerMap = _inputActions.FindActionMap("Player");
            _moveAction = playerMap.FindAction("Move");
            _lookAction = playerMap.FindAction("Look");
            _lmbAction = playerMap.FindAction("LMB");
            _rmbAction = playerMap.FindAction("RMB");
            _interactAction = playerMap.FindAction("Interact");
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