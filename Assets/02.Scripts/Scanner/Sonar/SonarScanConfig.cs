using UnityEngine;

namespace _02.Scripts.Sonar
{
    [CreateAssetMenu(fileName = "SonarScanConfig",  menuName = "Ater/Scanner/SonarScanConfig")]
    public class SonarScanConfig : ScriptableObject
    {
        [SerializeField] private float _scanRadius = 12f;
        [SerializeField] private float _scanAngle = 120f;

        [SerializeField] private float _cooldown = 1f;
        [SerializeField] private float _expandSpeed = 12f;
        [SerializeField] private float _trailDuration = 2f;

        [Header("Easing")] [SerializeField]
        private AnimationCurve _expandCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public float ScanRadius => _scanRadius;
        public float ScanAngle => _scanAngle;
        public float Cooldown => _cooldown;
        public float ExpandSpeed => _expandSpeed;
        public float TrailDuration => _trailDuration;
        public AnimationCurve ExpandCurve => _expandCurve;
    }
}