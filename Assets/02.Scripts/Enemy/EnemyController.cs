using UnityEngine;
using System.Collections;

namespace _02.Scripts.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyConfig _config;
        [SerializeField] private EnemyVisibilityChecker _visibilityChecker;

        private IEnemyAction _action;
        private Coroutine _deactivateCoroutine;
        
        public  EnemyConfig Config => _config;

        private void Awake()
        {
            _action = GetComponent<IEnemyAction>();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            _action.Execute(this);
        }

        public void RequestDeactivate()
        {
            _action.Stop();

            if (_deactivateCoroutine != null)
            {
                StopCoroutine(_deactivateCoroutine);
            }

            _deactivateCoroutine = StartCoroutine(DeactivateWhenNotVisible());
        }

        private IEnumerator DeactivateWhenNotVisible()
        {
            while (_visibilityChecker.IsVisibleToPlayer())
            {
                yield return null;
            }
            
            gameObject.SetActive(false);
            _deactivateCoroutine = null;
        }
    }
}