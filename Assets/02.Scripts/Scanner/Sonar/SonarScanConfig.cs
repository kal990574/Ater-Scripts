using UnityEngine;

namespace _02.Scripts.Sonar
{
    [CreateAssetMenu(fileName = "SonarScanConfig",  menuName = "Ater/Scanner/SonarScanConfig")]
    public class SonarScanConfig : ScriptableObject
    {
        [SerializeField] private float _scanRadius = 15f;
        [SerializeField] private float _scanAngle = 40f;

        [SerializeField] private float _cooldown = 1f;
        [SerializeField] private float _expandSpeed = 20f;
        [SerializeField] private float _trailDuration = 3f;

        public float ScanRadius => _scanRadius;
        public float ScanAngle => _scanAngle;
        public float Cooldown => _cooldown;
        public float ExpandSpeed => _expandSpeed;
        public float TrailDuration => _trailDuration;
    }
}