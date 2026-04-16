using UnityEngine;

namespace _02.Scripts.Enemy
{
    public class MannequinDeactivateZone : MonoBehaviour
    {
        [SerializeField] private EnemyController[] _mannequins;
        [SerializeField] private LayerMask _playerLayer;

        private bool _triggered;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered) return;
            if (((1 << other.gameObject.layer) & _playerLayer) == 0) return;

            _triggered = true;

            foreach (var mannequin in _mannequins)
            {
                if (mannequin != null && mannequin.gameObject.activeSelf)
                    mannequin.ForceDeactivate();
            }
        }
    }
}