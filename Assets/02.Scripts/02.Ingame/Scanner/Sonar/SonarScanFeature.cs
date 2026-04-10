using UnityEngine;
using System;

namespace _02.Scripts.Sonar
{
    public class SonarScanFeature : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SonarScanConfig _config;
        [SerializeField] private SonarCameraFeedback _cameraFeedback;
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private SonarScanEffect _effect;
        
        private GameEventPublisher _eventPublisher;
        private float _cooldownTimer;
        private int _currentCharges;
        private float _recoveryTimer;

        public bool IsCooltimeReady => _cooldownTimer <= 0f;
        public bool HaveResources =>  _currentCharges > 0;
        public float CooldownProgress => _cooldownTimer > 0f ? 1f - (_cooldownTimer / _config.Cooldown) : 1f;
        public int CurrentCharges => _currentCharges;
        public int MaxCharges => _config.MaxCharges;
        public event Action<int, int> OnChargesChanged;
        public ISoundService SoundService => SoundManager.Instance;
        
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
            
            _eventPublisher = new GameEventPublisher();
            _eventPublisher.SetSource(this);
            
            _cooldownTimer = 0;
            
            _currentCharges = _config.MaxCharges;
            _recoveryTimer = 0f;
        }
        
        
        public void UpdateCoolDown()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }

            if (_currentCharges < _config.MaxCharges)
            {
                _recoveryTimer += Time.deltaTime;
                if (_recoveryTimer >= _config.ChargeRecoveryTime)
                {
                    _recoveryTimer -= _config.ChargeRecoveryTime;
                    _currentCharges = Mathf.Min(_currentCharges + 1, _config.MaxCharges);
                    OnChargesChanged?.Invoke(_currentCharges, _config.MaxCharges);
                }
            }
        }

        public void TryScan()
        {
            if (_effect.IsScanning)
            {
                return;
            }
            
            if (!IsCooltimeReady)
            {
                SoundService.PlaySFX2D(_config.SonarCoolTime);
                return;
            }
            
            if (!HaveResources)
            {
                SoundService.PlaySFX2D(_config.SonarEmpty);
                return;
            }
            
            _cooldownTimer = _config.Cooldown;
            _currentCharges--;
            _recoveryTimer = 0f;
            OnChargesChanged?.Invoke(_currentCharges, _config.MaxCharges);
            if (_currentCharges == 0)
            {
                _eventPublisher.TryPublish(
                    context => new SonarScanEnergyDepletedRawEvent(context));
            }

            _effect.Play(transform.position, _cameraTarget.forward);
            _cameraFeedback.Play();
            SoundService.PlaySFX2D(_config.SonarActive);
            _eventPublisher.TryPublish(
                context => new SonarScanStartedRawEvent(
                    context,
                    transform.position,
                    _cameraTarget.forward,
                    _config.ExpandSpeed,
                    _config.ScanRadius,
                    _config.ScanAngle));
        }
    }
}
