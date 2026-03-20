using UnityEngine;

namespace _02.Scripts.Enemy
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Ater/Enemy/Config")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("Chase")]
        [SerializeField] private float _chaseSpeed = 3f;
        [SerializeField] private float _chaseTimeout = 15f;
        
        [Header("Kill")]
        [SerializeField] private float _killRange = 1.5f;
        
        public float ChaseSpeed => _chaseSpeed;
        public float ChaseTimeout => _chaseTimeout;
        public float KillRange => _killRange;
    }
}