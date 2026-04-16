using UnityEngine;

namespace _02.Scripts.Enemy
{
    public class MannequinDeactivateZone : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayer;

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _enemyLayer) == 0) return;

            var controller = other.GetComponentInParent<EnemyController>();
            if (controller != null)
                controller.ForceDeactivate();
        }
    }
}