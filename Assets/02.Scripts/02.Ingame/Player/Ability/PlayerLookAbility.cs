using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace _02.Scripts.Player
{
    
    public class PlayerLookAbility : PlayerAbility
    {
        [SerializeField] private Transform _cameraTarget;

        private IPlayerInput _input;
        private PlayerConfigSO _config;
        private float _pitch;
        private float _sensitivity;

        private void Start()
        {
            _input = GetComponent<IPlayerInput>();
            _config = _owner.Config;
            SensitivitySetting.Init(_config.MouseSensitivity);
            _sensitivity = SensitivitySetting.Sensitivity;
            SensitivitySetting.OnSensitivityChanged += SetSensitivity;
            LookCursor();
        }

        private void OnDestroy()
        {
            SensitivitySetting.OnSensitivityChanged -= SetSensitivity;
        }

        public void SetSensitivity(float sensitivity)
        {
            _sensitivity = sensitivity;
        }

        private void LateUpdate()
        {
            if(!_owner.CanRotate)
            {
                return;
            }
            Rotate();
        }

        private void Rotate()
        {
            Vector2 lookInput = _input.LookInput;

            float yaw = lookInput.x * _sensitivity;
            transform.Rotate(Vector3.up, yaw);
            
            _pitch -= lookInput.y * _sensitivity;
            _pitch = Mathf.Clamp(_pitch, _config.MinPitch, _config.MaxPitch);
            _cameraTarget.localRotation = Quaternion.Euler(_pitch, 0, 0);
        }

        private void LookCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}