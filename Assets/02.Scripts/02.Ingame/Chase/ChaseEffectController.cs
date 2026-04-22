using System.Collections;
using _02.Scripts.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace _02.Scripts.Chase
{
    public class ChaseEffectController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private CinemachineCamera _virtualCamera;

        [Header("Audio")]
        [SerializeField] private SoundKeyReference _breathingKey;
        [SerializeField] private SoundKeyReference _heartbeatKey;

        [Header("FOV")]
        [SerializeField] private float _fovOffset = 10f;
        [SerializeField] private float _fovLerpDuration = 0.5f;

        [Header("Head Bob")]
        [SerializeField] private float _headBobFrequency = 10f;
        [SerializeField] private float _headBobAmplitude = 3f;

        [Header("Camera Damping")]
        [SerializeField, Range(0.01f, 1f)] private float _cameraDampingFactor = 0.1f;

        private float _baseFov;
        private bool _isActive;
        private AudioSource _breathingSource;
        private AudioSource _heartbeatSource;
        private Coroutine _fovCoroutine;

        private void Awake()
        {
            _baseFov = _virtualCamera.Lens.FieldOfView;
        }

        public void StartChaseEffects()
        {
            if (_isActive) return;
            _isActive = true;

            // 루프 SFX 시작
            Vector3 playerPos = _playerController.transform.position;

            if (!_breathingKey.IsEmpty)
                _breathingSource = SoundManager.Instance.PlayLoopSFX(_breathingKey, playerPos);

            if (!_heartbeatKey.IsEmpty)
                _heartbeatSource = SoundManager.Instance.PlayLoopSFX(_heartbeatKey, playerPos);

            // 발소리 전환
            var footstep = _playerController.GetAbility<PlayerFootstepAbility>();
            if (footstep != null)
                footstep.SetChasing(true);

            // Head Bob + Damping
            var look = _playerController.GetAbility<PlayerLookAbility>();
            if (look != null)
                look.SetChaseMode(true, _headBobFrequency, _headBobAmplitude, _cameraDampingFactor);

            // FOV 확장
            StartFovLerp(_baseFov + _fovOffset);
        }

        public void StopChaseEffects()
        {
            if (!_isActive) return;
            _isActive = false;

            // 루프 SFX 정지
            if (_breathingSource != null)
            {
                SoundManager.Instance.StopLoopSFX(_breathingSource);
                _breathingSource = null;
            }

            if (_heartbeatSource != null)
            {
                SoundManager.Instance.StopLoopSFX(_heartbeatSource);
                _heartbeatSource = null;
            }

            // 발소리 복귀
            var footstep = _playerController.GetAbility<PlayerFootstepAbility>();
            if (footstep != null)
                footstep.SetChasing(false);

            // Head Bob + Damping 해제
            var look = _playerController.GetAbility<PlayerLookAbility>();
            if (look != null)
                look.SetChaseMode(false, 0f, 0f, 0f);

            // FOV 복귀
            StartFovLerp(_baseFov);
        }

        private void Update()
        {
            if (!_isActive) return;

            Vector3 pos = _playerController.transform.position;

            if (_breathingSource != null)
                _breathingSource.transform.position = pos;

            if (_heartbeatSource != null)
                _heartbeatSource.transform.position = pos;
        }

        private void StartFovLerp(float targetFov)
        {
            if (_fovCoroutine != null)
                StopCoroutine(_fovCoroutine);

            _fovCoroutine = StartCoroutine(LerpFov(targetFov));
        }

        private IEnumerator LerpFov(float targetFov)
        {
            float startFov = _virtualCamera.Lens.FieldOfView;
            float elapsed = 0f;

            while (elapsed < _fovLerpDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fovLerpDuration;
                var lens = _virtualCamera.Lens;
                lens.FieldOfView = Mathf.Lerp(startFov, targetFov, t);
                _virtualCamera.Lens = lens;
                yield return null;
            }

            var finalLens = _virtualCamera.Lens;
            finalLens.FieldOfView = targetFov;
            _virtualCamera.Lens = finalLens;
            _fovCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_fovCoroutine != null)
                StopCoroutine(_fovCoroutine);

            if (_breathingSource != null)
                SoundManager.Instance.StopLoopSFX(_breathingSource);

            if (_heartbeatSource != null)
                SoundManager.Instance.StopLoopSFX(_heartbeatSource);
        }
    }
}