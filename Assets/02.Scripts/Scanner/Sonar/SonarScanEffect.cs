using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace _02.Scripts.Sonar
{
    public class SonarScanEffect : MonoBehaviour
    {
        [SerializeField] private Material _scanMaterial;

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
        private static readonly int _ringFillIntensityId = Shader.PropertyToID("_RingFillIntensity");
        private static readonly int _ringGradientPowerId = Shader.PropertyToID("_RingGradientPower");
        private static readonly int _ringOpacityId = Shader.PropertyToID("_RingOpacity");

        
        private SonarScanConfig _config;
        private float _currentRadius;
        private float _trailFadeRadius;
        private float _ringOpacity;
        private bool _isScanning;
        private Vector3 _scanOrigin;
        private Vector3 _scanDirection;
        private Coroutine _scanCoroutine;
        public bool IsScanning => _isScanning;

        public void Init(SonarScanConfig config = null)
        {
            if (config != null)
            {
                _config = config;
            }

            _currentRadius = 0f;
            _trailFadeRadius = 0f;
            _ringOpacity = 0f;
            _isScanning = false;
            UpdateMaterialProperties();
            _scanCoroutine = null;
        }

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
            _ringOpacity = 1f;

            float maxRadius = _config.ScanRadius;
            float totalDistance = maxRadius + _config.RingWidth;
            float expandDuration = totalDistance / _config.ExpandSpeed;
            float trailDelay = _config.TrailDuration;
            float elapsed = 0f;

            while (_ringOpacity > 0f || _trailFadeRadius < maxRadius)
            {
                elapsed += Time.deltaTime;

                // 링 확장 (0 → maxRadius + ringWidth, 커브 하나로 전체 이동)
                // 셰이더가 maxRadius 밖을 클리핑 → 끝부분에서 얇아지다 자연 소멸
                if (_ringOpacity > 0f)
                {
                    float t = Mathf.Clamp01(elapsed / expandDuration);
                    _currentRadius = _config.ExpandCurve.Evaluate(t) * totalDistance;

                    if (t >= 1f)
                    {
                        _currentRadius = maxRadius;
                        _ringOpacity = 0f;
                    }
                }

                // 잔상 소멸: trailDelay 후 trailFadeSpeed로 안쪽부터 지움
                if (elapsed > trailDelay)
                {
                    _trailFadeRadius += _config.TrailFadeSpeed * Time.deltaTime;
                    _trailFadeRadius = Mathf.Min(_trailFadeRadius, maxRadius);
                }

                UpdateMaterialProperties();
                yield return null;
            }

            _currentRadius = 0f;
            _trailFadeRadius = 0f;
            _ringOpacity = 0f;
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
            _scanMaterial.SetFloat(_ringWidthId, _config.RingWidth);
            _scanMaterial.SetColor(_scanColorId, _config.ScanColor);
            _scanMaterial.SetFloat(_edgeThresholdId, _config.EdgeThreshold);
            _scanMaterial.SetFloat(_scanLineFrequencyId, _config.ScanLineFrequency);
            _scanMaterial.SetFloat(_trailIntensityId, _config.TrailIntensity);
            _scanMaterial.SetFloat(_trailFadeRadiusId, _trailFadeRadius);
            _scanMaterial.SetFloat(_ringFillIntensityId, _config.RingFillIntensity);
            _scanMaterial.SetFloat(_ringGradientPowerId, _config.RingGradientPower);
            _scanMaterial.SetFloat(_ringOpacityId, _ringOpacity);
        }
    }
}