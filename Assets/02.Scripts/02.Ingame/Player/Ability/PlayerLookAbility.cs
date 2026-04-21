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

        // 추격 모드
        private bool _isChaseMode;
        private float _headBobFrequency;
        private float _headBobAmplitude;
        private float _dampingFactor;
        private float _headBobTimer;
        private float _targetYaw;
        private float _currentDampedYaw;

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

            if (_isChaseMode && _dampingFactor > 0f)
            {
                _targetYaw += yaw;
                _currentDampedYaw = Mathf.Lerp(_currentDampedYaw, _targetYaw, _dampingFactor);
                transform.rotation = Quaternion.Euler(0f, _currentDampedYaw, 0f);
            }
            else
            {
                transform.Rotate(Vector3.up, yaw);
            }

            _pitch -= lookInput.y * _sensitivity;
            _pitch = Mathf.Clamp(_pitch, _config.MinPitch, _config.MaxPitch);
            _cameraTarget.localRotation = Quaternion.Euler(_pitch, 0, 0);

            if (_isChaseMode && _input.MoveInput.magnitude > 0.1f)
            {
                _headBobTimer += Time.deltaTime * _headBobFrequency;
                float bobX = Mathf.Sin(_headBobTimer) * _headBobAmplitude;
                float bobZ = Mathf.Sin(_headBobTimer * 0.6f) * _headBobAmplitude * 0.5f;
                _cameraTarget.localRotation *= Quaternion.Euler(bobX, 0f, bobZ);
            }
        }

        public void SetChaseMode(bool active, float bobFreq, float bobAmp, float dampFactor)
        {
            _isChaseMode = active;
            _headBobFrequency = bobFreq;
            _headBobAmplitude = bobAmp;
            _dampingFactor = dampFactor;
            _headBobTimer = 0f;

            if (active)
            {
                _targetYaw = transform.eulerAngles.y;
                _currentDampedYaw = _targetYaw;
            }
        }

        private void LookCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}