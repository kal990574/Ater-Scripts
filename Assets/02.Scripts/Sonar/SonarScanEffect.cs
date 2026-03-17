using UnityEngine;
using System.Collections;

namespace _02.Scripts.Sonar
{
    public class SonarScanEffect : MonoBehaviour
    {
        [SerializeField] private SonarScanConfig _config;
        [SerializeField] private Material _scanMaterial;

        [SerializeField] private float _ringWidth = 2f;
        [SerializeField] private Color _scanColor = new(0.4f, 0.7f, 1.0f, 1.0f);
        [SerializeField] private float _edgeThreshold = 0.1f;
        [SerializeField] private float _scanLineFrequency = 50f;
        [SerializeField] private float _trailIntensity = 0.3f;

        private static readonly int _scanOriginId = Shader.PropertyToID("_ScanOrigin");
        private static readonly int _scanDirectionId = Shader.PropertyToID("_ScanDirection");
        private static readonly int _scanRadiusId = Shader.PropertyToID("_ScanRadius");
        private static readonly int _scanMaxRadiusId = Shader.PropertyToID("_ScanMaxRadius");
        private static readonly int _scanAngleId = Shader.PropertyToID("_ScanAngle");
        private static readonly int _ringWidthId = Shader.PropertyToID("_RingWidth");
        private static readonly int _scanColorId = Shader.PropertyToID("_ScanColor");
        private static readonly int _edgeThresholdId = Shader.PropertyToID("_EdgeThreshold");
        private static readonly int _scanLineFrequencyId = Shader.PropertyToID("_ScanLineFrequency");
        private static readonly int _trailIntensityId = Shader.PropertyToID("_TrailIntensity");
        private static readonly int _trailFadeRadiusId = Shader.PropertyToID("_TrailFadeRadius");

        private float _currentRadius;
        private float _trailFadeRadius;
        private bool _isScanning;
        private Vector3 _scanOrigin;
        private Vector3 _scanDirection;
        private Coroutine _scanCoroutine;

        public bool IsScanning => _isScanning;

        public void Play(Vector3 origin, Vector3 direction)
        {
            if (_scanCoroutine != null)
            {
                StopCoroutine(_scanCoroutine);
            }

            _scanOrigin = origin;
            _scanDirection = direction;
            _scanCoroutine = StartCoroutine(ScanCoroutine());
        }

        private IEnumerator ScanCoroutine()
        {
            _isScanning = true;
            _currentRadius = 0f;
            _trailFadeRadius = 0f;
            float maxRadius = _config.ScanRadius;
            float expandSpeed = _config.ExpandSpeed;

            // 1단계: 링 확장 + 잔상 지연 추격
            float trailDelay = _config.TrailDuration;
            float elapsed = 0f;

            while (_currentRadius < maxRadius || _trailFadeRadius < maxRadius)
            {
                elapsed += Time.deltaTime;

                // 링 확장
                if (_currentRadius < maxRadius)
                {
                    _currentRadius += expandSpeed * Time.deltaTime;
                    _currentRadius = Mathf.Min(_currentRadius, maxRadius);
                }

                // 잔상 소멸 경계: trailDelay 후 동일 속도로 추격
                if (elapsed > trailDelay)
                {
                    _trailFadeRadius += expandSpeed * Time.deltaTime;
                    _trailFadeRadius = Mathf.Min(_trailFadeRadius, maxRadius);
                }

                UpdateMaterialProperties();
                yield return null;
            }

            // 초기화
            _currentRadius = 0f;
            _trailFadeRadius = 0f;
            _isScanning = false;
            UpdateMaterialProperties();
            _scanCoroutine = null;
        }

        private void UpdateMaterialProperties()
        {
            _scanMaterial.SetVector(_scanOriginId, _scanOrigin);
            _scanMaterial.SetVector(_scanDirectionId, _scanDirection);
            _scanMaterial.SetFloat(_scanRadiusId, _currentRadius);
            _scanMaterial.SetFloat(_scanMaxRadiusId, _config.ScanRadius);
            _scanMaterial.SetFloat(_scanAngleId, _config.ScanAngle);
            _scanMaterial.SetFloat(_ringWidthId, _ringWidth);
            _scanMaterial.SetColor(_scanColorId, _scanColor);
            _scanMaterial.SetFloat(_edgeThresholdId, _edgeThreshold);
            _scanMaterial.SetFloat(_scanLineFrequencyId, _scanLineFrequency);
            _scanMaterial.SetFloat(_trailIntensityId, _trailIntensity);
            _scanMaterial.SetFloat(_trailFadeRadiusId, _trailFadeRadius);
        }
    }
}