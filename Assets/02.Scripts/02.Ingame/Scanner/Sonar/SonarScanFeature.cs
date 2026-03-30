using UnityEngine;
using _02.Scripts.Player;

namespace _02.Scripts.Sonar
{
    public class SonarScanFeature : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SonarScanConfig _config;
        [SerializeField] private SonarCameraFeedback _cameraFeedback;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private SonarScanEffect _effect;
        
       
        private float _cooldownTimer;
        
        public bool IsReady => _cooldownTimer <= 0f;
        public float CooldownProgress => _cooldownTimer > 0f ? 1f - (_cooldownTimer / _config.Cooldown) : 1f;

        public void Initialize()
        {
            if (_config == null)
            {
                Debug.LogError($"[{nameof(SonarScanFeature)}] Sonar Config is Missing.", this);
                return;
            }
            
            if (TryGetComponent(out SonarScanEffect effect))
            {
                _effect = effect;
                _effect.Init(_config);
            }
            _cooldownTimer = 0;
        }
        
        
        public void UpdateCoolDown()
        {
            if (_cooldownTimer > 0f)
            {
                Debug.Log($"충전중 {CooldownProgress}");
                _cooldownTimer -= Time.deltaTime;
            }
        }

        public void TryScan()
        {
            if (!IsReady) return;
            if (_effect.IsScanning) return;
            
            _cooldownTimer = _config.Cooldown;
            _effect.Play(transform.position, _cameraTarget.forward);
            _cameraFeedback.Play();
        }
    }
}