using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

namespace _02.Scripts.Sonar
{
    public class SonarCameraFeedback : MonoBehaviour
    {
        [Header("Camera Shake")]
        [SerializeField] private float _shakeIntensity = 0.05f;
        [SerializeField] private float _shakeDuration = 0.15f;

        [Header("FOV Punch")]
        [SerializeField] private CinemachineCamera _virtualCamera;
        [SerializeField] private float _fovPunchAmount = 3f;
        [SerializeField] private float _fovPunchDuration = 0.2f;

        private Coroutine _shakeCoroutine;
        private Coroutine _fovCoroutine;
        private float _baseFov;
        private Vector3 _baseLocalPosition;

        private void Start()
        {
            _baseLocalPosition = transform.localPosition;
            if (_virtualCamera != null)
            {
                _baseFov = _virtualCamera.Lens.FieldOfView;
            }
        }

        public void Play()
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                transform.localPosition = _baseLocalPosition;
            }

            _shakeCoroutine = StartCoroutine(ShakeCoroutine());

            if (_virtualCamera != null)
            {
                if (_fovCoroutine != null)
                {
                    StopCoroutine(_fovCoroutine);
                }

                _fovCoroutine = StartCoroutine(FovPunchCoroutine());
            }
        }

        private IEnumerator ShakeCoroutine()
        {
            float elapsed = 0f;

            while (elapsed < _shakeDuration)
            {
                elapsed += Time.deltaTime;
                float t = 1f - (elapsed / _shakeDuration);
                Vector3 offset = Random.insideUnitSphere * (_shakeIntensity * t);
                transform.localPosition = _baseLocalPosition + offset;
                yield return null;
            }

            transform.localPosition = _baseLocalPosition;
            _shakeCoroutine = null;
        }

        private IEnumerator FovPunchCoroutine()
        {
            float elapsed = 0f;
            var lens = _virtualCamera.Lens;

            while (elapsed < _fovPunchDuration)
            {
                elapsed += Time.deltaTime;
                float t = 1f - (elapsed / _fovPunchDuration);
                t = t * t;
                lens.FieldOfView = _baseFov + _fovPunchAmount * t;
                _virtualCamera.Lens = lens;
                yield return null;
            }

            lens.FieldOfView = _baseFov;
            _virtualCamera.Lens = lens;
            _fovCoroutine = null;
        }
    }
}