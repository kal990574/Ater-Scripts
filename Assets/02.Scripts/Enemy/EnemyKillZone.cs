using UnityEngine;

namespace _02.Scripts.Enemy
{
    public class EnemyKillZone : MonoBehaviour
    {
        [SerializeField] private LayerMask _playerLayer;

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _playerLayer) == 0) return;
            
            // TODO: 게임 오버 시스템 연동
            Debug.Log("Game Over - 적에게 잡혔습니다");
        }
    }
}