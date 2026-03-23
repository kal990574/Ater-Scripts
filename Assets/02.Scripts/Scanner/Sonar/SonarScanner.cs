using UnityEngine;
using _02.Scripts.Player;

namespace _02.Scripts.Sonar
{
    public class SonarScanner : MonoBehaviour
    {
        [SerializeField] private SonarScanConfig _config;
        [SerializeField] private SonarScanEffect _effect;
        [SerializeField] private SonarCameraFeedback _cameraFeedback;
        [SerializeField] private Transform _cameraTarget;

        private IPlayerInput _input;
        private float _cooldownTimer;
        
        public bool IsReady => _cooldownTimer <= 0f;
        public float CooldownProgress => _cooldownTimer > 0f ? 1f - (_cooldownTimer / _config.Cooldown) : 1f;

        private void Start()
        {
            _input = GetComponentInParent<IPlayerInput>();
        }

        private void Update()
        {
            UpdateCoolDown();
            if (_input.RmbPressInput)
            {
                TryScan();
            }
        }

        private void UpdateCoolDown()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }
        }

        private void TryScan()
        {
            if (!IsReady) return;
            if (_effect.IsScanning) return;
            
            _cooldownTimer = _config.Cooldown;
            _effect.Play(transform.position, _cameraTarget.forward);
            _cameraFeedback.Play();
        }
    }
}