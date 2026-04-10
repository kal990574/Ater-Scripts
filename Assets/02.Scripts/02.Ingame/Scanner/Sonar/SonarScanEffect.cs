using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace _02.Scripts.Sonar
{
    public class SonarScanEffect : MonoBehaviour
    {
        [SerializeField] private Material _scanMaterial;

        private const int MAX_RINGS = 4;

        private static readonly int _scanMaxRadiusId = Shader.PropertyToID("_ScanMaxRadius");
        private static readonly int _scanAngleId = Shader.PropertyToID("_ScanAngle");
        private static readonly int _ringWidthId = Shader.PropertyToID("_RingWidth");
        private static readonly int _scanColorId = Shader.PropertyToID("_ScanColor");
        private static readonly int _edgeThresholdId = Shader.PropertyToID("_EdgeThreshold");
        private static readonly int _trailIntensityId = Shader.PropertyToID("_TrailIntensity");
        private static readonly int _ringFillIntensityId = Shader.PropertyToID("_RingFillIntensity");
        private static readonly int _ringGradientPowerId = Shader.PropertyToID("_RingGradientPower");

        private static readonly int _ringOriginsId = Shader.PropertyToID("_RingOrigins");
        private static readonly int _ringDirectionsId = Shader.PropertyToID("_RingDirections");
        private static readonly int _ringParamsId = Shader.PropertyToID("_RingParams");
        private static readonly int _ringCountId = Shader.PropertyToID("_RingCount");

        private class RingState
        {
            public Vector3 Origin;
            public Vector3 Direction;
            public float Radius;
            public float TrailFadeRadius;
            public float Opacity;
            public float Elapsed;
        }

        private SonarScanConfig _config;
        private readonly List<RingState> _rings = new(MAX_RINGS);

        // 셰이더 업로드용 재사용 배열 (매 프레임 new 방지)
        private readonly Vector4[] _ringOriginsBuf = new Vector4[MAX_RINGS];
        private readonly Vector4[] _ringDirectionsBuf = new Vector4[MAX_RINGS];
        private readonly Vector4[] _ringParamsBuf = new Vector4[MAX_RINGS];

        public bool IsScanning => _rings.Count > 0;
        public bool IsFull => _rings.Count >= MAX_RINGS;

        public void Init(SonarScanConfig config = null)
        {
            if (config != null)
            {
                _config = config;
            }

            _rings.Clear();
            UploadMaterialProperties();
        }

        public void Play(Vector3 origin, Vector3 direction)
        {
            // 슬롯 포화 시 가장 오래된 링 드롭 (A안)
            if (_rings.Count >= MAX_RINGS)
            {
                _rings.RemoveAt(0);
            }

            _rings.Add(new RingState
            {
                Origin = origin,
                Direction = direction,
                Radius = 0f,
                TrailFadeRadius = 0f,
                Opacity = 1f,
                Elapsed = 0f,
            });
        }

        private void Update()
        {
            if (_config == null) return;

            if (_rings.Count == 0)
            {
                // Frag 빠른 경로 보장 위해 한 번은 0 업로드
                _scanMaterial.SetInt(_ringCountId, 0);
                return;
            }

            float maxRadius = _config.ScanRadius;
            float totalDistance = maxRadius + _config.RingWidth;
            float expandDuration = totalDistance / _config.ExpandSpeed;
            float trailDelay = _config.TrailDuration;
            float dt = Time.deltaTime;

            for (int i = _rings.Count - 1; i >= 0; i--)
            {
                var r = _rings[i];
                r.Elapsed += dt;

                // 링 확장 (0 → maxRadius + ringWidth, 커브 하나로 전체 이동)
                if (r.Opacity > 0f)
                {
                    float t = Mathf.Clamp01(r.Elapsed / expandDuration);
                    r.Radius = _config.ExpandCurve.Evaluate(t) * totalDistance;

                    if (t >= 1f)
                    {
                        r.Radius = maxRadius;
                        r.Opacity = 0f;
                    }
                }

                // 잔상 소멸: trailDelay 후 trailFadeSpeed로 안쪽부터 지움
                if (r.Elapsed > trailDelay)
                {
                    r.TrailFadeRadius = Mathf.Min(
                        r.TrailFadeRadius + _config.TrailFadeSpeed * dt,
                        maxRadius);
                }

                // 수명 종료 판정
                bool alive = r.Opacity > 0f || r.TrailFadeRadius < maxRadius;
                if (!alive)
                {
                    _rings.RemoveAt(i);
                }
            }

            UploadMaterialProperties();
        }

        private void UploadMaterialProperties()
        {
            if (_config == null || _scanMaterial == null) return;

            // 전역 상수
            _scanMaterial.SetFloat(_scanMaxRadiusId, _config.ScanRadius);
            _scanMaterial.SetFloat(_scanAngleId, _config.ScanAngle);
            _scanMaterial.SetFloat(_ringWidthId, _config.RingWidth);
            _scanMaterial.SetColor(_scanColorId, _config.ScanColor);
            _scanMaterial.SetFloat(_edgeThresholdId, _config.EdgeThreshold);
            _scanMaterial.SetFloat(_trailIntensityId, _config.TrailIntensity);
            _scanMaterial.SetFloat(_ringFillIntensityId, _config.RingFillIntensity);
            _scanMaterial.SetFloat(_ringGradientPowerId, _config.RingGradientPower);

            // 링 배열: 항상 MAX_RINGS 크기로 업로드 (SetVectorArray 크기 고정 특성)
            for (int i = 0; i < MAX_RINGS; i++)
            {
                if (i < _rings.Count)
                {
                    var r = _rings[i];
                    _ringOriginsBuf[i] = new Vector4(r.Origin.x, r.Origin.y, r.Origin.z, 0f);
                    _ringDirectionsBuf[i] = new Vector4(r.Direction.x, r.Direction.y, r.Direction.z, 0f);
                    _ringParamsBuf[i] = new Vector4(r.Radius, r.TrailFadeRadius, r.Opacity, 0f);
                }
                else
                {
                    _ringOriginsBuf[i] = Vector4.zero;
                    _ringDirectionsBuf[i] = Vector4.zero;
                    _ringParamsBuf[i] = Vector4.zero;
                }
            }

            _scanMaterial.SetVectorArray(_ringOriginsId, _ringOriginsBuf);
            _scanMaterial.SetVectorArray(_ringDirectionsId, _ringDirectionsBuf);
            _scanMaterial.SetVectorArray(_ringParamsId, _ringParamsBuf);
            _scanMaterial.SetInt(_ringCountId, _rings.Count);
        }
    }
}
