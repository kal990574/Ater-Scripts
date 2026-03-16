using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace _02.Scripts.Player
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private float _mouseSensitivity = 0.15f;

        [SerializeField] private float _minPitch = -90f;
        [SerializeField] private float _maxPitch = 90f;

        [SerializeField] private Transform _cameraTarget;

        private IPlayerInput _input;
        private float _pitch;

        private void Start()
        {
            _input = GetComponent<IPlayerInput>();
            LookCursor();
        }
        
        private void LateUpdate()
        {
            Rotate();
        }

        private void Rotate()
        {
            Vector2 lookInput = _input.LookInput;

            float yaw = lookInput.x * _mouseSensitivity;
            transform.Rotate(Vector3.up, yaw);
            
            _pitch -= lookInput.y * _mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
            _cameraTarget.localRotation = Quaternion.Euler(_pitch, 0, 0);
        }

        private void LookCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}