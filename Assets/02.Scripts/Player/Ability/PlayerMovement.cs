using UnityEngine;

namespace _02.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : PlayerAbility
    {
        private CharacterController _controller;
        private IPlayerInput _input;
        private PlayerConfigSO _config;
        private float _verticalVelocity;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<IPlayerInput>();
            
            _config = _owner.Config;
        }

        private void Update()
        {
            if (!_owner.CanMove)
            {
                return;
            }
            
            ApplyGravity();
            Move();
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }
            _verticalVelocity += _config.Gravity * Time.deltaTime;
        }

        private void Move()
        {
            Vector2 input = _input.MoveInput;
            Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
            
            float speed = input.y < 0f ? _config.MoveSpeed *  _config.BackwardSpeedMultiplier :  _config.MoveSpeed;

            Vector3 velocity = moveDirection * speed;
            velocity.y = _verticalVelocity;
            
            _controller.Move(velocity * Time.deltaTime);
        }
    }
}