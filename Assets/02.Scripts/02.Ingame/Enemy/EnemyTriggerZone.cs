using UnityEngine;

namespace _02.Scripts.Enemy
{
    public class EnemyTriggerZone : MonoBehaviour
    {
        [SerializeField] private EnemyController _enemyController;
        [SerializeField] private LayerMask _playerLayer;

        private bool _triggered;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered) return;
            if (((1 << other.gameObject.layer) & _playerLayer) == 0) return;

            _triggered = true;
            _enemyController.Activate();
        }
    }
}