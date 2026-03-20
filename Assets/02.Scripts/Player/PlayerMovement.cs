using UnityEngine;

namespace _02.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 3.5f;
        [SerializeField] [Range(0f, 1f)] private float _backwardSpeedMultiplier = 0.5f;
        [SerializeField] private float _gravity = -9.81f;
        
        private CharacterController _controller;
        private IPlayerInput _input;
        private float _verticalVelocity;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<IPlayerInput>();
        }

        private void Update()
        {
            ApplyGravity();
            Move();
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }
            _verticalVelocity += _gravity * Time.deltaTime;
        }

        private void Move()
        {
            Vector2 input = _input.MoveInput;
            Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
            
            float speed = input.y < 0f ? _moveSpeed * _backwardSpeedMultiplier : _moveSpeed;

            Vector3 velocity = moveDirection * speed;
            velocity.y = _verticalVelocity;
            
            _controller.Move(velocity * Time.deltaTime);
        }
    }
}