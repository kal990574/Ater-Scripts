using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _02.Scripts.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ChaseAction : MonoBehaviour, IEnemyAction
    {
        [SerializeField] private Transform _target;
        
        private NavMeshAgent _agent;
        private EnemyController _controller;
        private Coroutine _chaseCoroutine;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Execute(EnemyController enemy)
        {
            _controller = enemy;
            _agent.speed = enemy.Config.ChaseSpeed;
            _agent.isStopped = false;
            _chaseCoroutine = StartCoroutine(ChaseCoroutine(enemy.Config.ChaseTimeout));
        }

        public void Stop()
        {
            if (_chaseCoroutine != null)
            {
                StopCoroutine(_chaseCoroutine);
                _chaseCoroutine = null;
            }
            
            _agent.isStopped = true;
        }

        private IEnumerator ChaseCoroutine(float timeout)
        {
            float elapsed = 0f;

            while (elapsed < timeout)
            {
                _agent.SetDestination(_target.position);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            _chaseCoroutine = null;
            _controller.RequestDeactivate();
        }
    }
}