using UnityEngine;
using UnityEngine.InputSystem;

namespace _02.Scripts.Player
{
    public class PlayerInputHandler : MonoBehaviour, IPlayerInput
    {
        [SerializeField] private InputActionAsset _inputActions;
        
        private InputAction _moveAction;
        private InputAction _lookAction;
        
        public Vector2 MoveInput => _moveAction.ReadValue<Vector2>();
        public Vector2 LookInput => _lookAction.ReadValue<Vector2>();

        private void Awake()
        {
            InputActionMap playerMap = _inputActions.FindActionMap("Player");
            _moveAction = playerMap.FindAction("Move");
            _lookAction = playerMap.FindAction("Look");
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