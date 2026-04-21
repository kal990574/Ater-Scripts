using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _02.Scripts.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ChaseAction : MonoBehaviour, IEnemyAction
    {
        [SerializeField] private Transform _target;
        [SerializeField] private SoundKeyReference _chaseSfxKey;

        private NavMeshAgent _agent;
        private EnemyController _controller;
        private Coroutine _chaseCoroutine;
        private AudioSource _chaseSfxSource;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Execute(EnemyController enemy)
        {
            _controller = enemy;
            _agent.enabled = true;
            _agent.speed = enemy.Config.ChaseSpeed;
            _agent.isStopped = false;

            if (!_chaseSfxKey.IsEmpty)
                _chaseSfxSource = SoundManager.Instance.PlayLoopSFX(_chaseSfxKey, transform.position);

            _chaseCoroutine = StartCoroutine(ChaseCoroutine(enemy.Config.ChaseTimeout));
        }

        public void Stop()
        {
            if (_chaseSfxSource != null)
            {
                SoundManager.Instance.StopLoopSFX(_chaseSfxSource);
                _chaseSfxSource = null;
            }

            if (_chaseCoroutine != null)
            {
                StopCoroutine(_chaseCoroutine);
                _chaseCoroutine = null;
            }

            if (_agent.isOnNavMesh)
                _agent.isStopped = true;
        }

        private IEnumerator ChaseCoroutine(float timeout)
        {
            float elapsed = 0f;

            while (elapsed < timeout)
            {
                _agent.SetDestination(_target.position);

                if (_chaseSfxSource != null)
                    _chaseSfxSource.transform.position = transform.position;

                elapsed += Time.deltaTime;
                yield return null;
            }
            
            _chaseCoroutine = null;
            _controller.RequestDeactivate();
        }
    }
}