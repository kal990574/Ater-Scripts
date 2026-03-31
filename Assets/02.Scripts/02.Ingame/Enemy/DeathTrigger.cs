using UnityEngine;
using _02.Scripts.Core;
using _02.Scripts.Core.Domain;

namespace _02.Scripts.Enemy
{
    public class DeathTrigger : MonoBehaviour
    {
        [SerializeField] private LayerMask _playerLayer;

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _playerLayer) == 0) return;
            
            Debug.Log("Game Over - 적에게 잡혔습니다");
            Managers.Get<IGameManager>().GameOver();
        }
    }
}