using UnityEngine;

namespace _02.Scripts.Sonar
{
    [CreateAssetMenu(fileName = "SonarScanConfig",  menuName = "Ater/Scanner/SonarScanConfig")]
    public class SonarScanConfig : ScriptableObject
    {
        [Header("Scan")]
        [SerializeField] private float _scanRadius = 12f;
        [SerializeField] private float _scanAngle = 120f;
        [SerializeField] private float _cooldown = 1f;
        [SerializeField] private float _expandSpeed = 12f;
        [SerializeField] private float _trailDuration = 2f;

        [Header("Easing")]
        [SerializeField] private AnimationCurve _expandCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Visual")]
        [SerializeField] private float _ringWidth = 2f;
        [SerializeField] private Color _scanColor = new(0.4f, 0.7f, 1.0f, 1.0f);
        [SerializeField] private float _edgeThreshold = 0.1f;
        [SerializeField] private float _scanLineFrequency = 50f;
        [SerializeField] private float _trailIntensity = 0.3f;
        [SerializeField] [Range(0f, 1f)] private float _ringFillIntensity = 0.4f;
        [SerializeField] private float _ringFadeDuration = 0.5f;

        public float ScanRadius => _scanRadius;
        public float ScanAngle => _scanAngle;
        public float Cooldown => _cooldown;
        public float ExpandSpeed => _expandSpeed;
        public float TrailDuration => _trailDuration;
        public AnimationCurve ExpandCurve => _expandCurve;

        public float RingWidth => _ringWidth;
        public Color ScanColor => _scanColor;
        public float EdgeThreshold => _edgeThreshold;
        public float ScanLineFrequency => _scanLineFrequency;
        public float TrailIntensity => _trailIntensity;
        public float RingFillIntensity => _ringFillIntensity;
        public float RingFadeDuration => _ringFadeDuration;
    }
}