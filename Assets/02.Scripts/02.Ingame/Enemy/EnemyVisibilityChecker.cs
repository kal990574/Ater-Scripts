using UnityEngine;

namespace _02.Scripts.Enemy
{
    public class EnemyVisibilityChecker : MonoBehaviour
    {
        [SerializeField] private LayerMask _obstacleLayer;

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        public bool IsVisibleToPlayer()
        {
            Vector3 viewportPoint = _mainCamera.WorldToViewportPoint(transform.position);
            bool inViewport = viewportPoint.z > 0f && viewportPoint.x > 0f && viewportPoint.x < 1f &&
                              viewportPoint.y > 0f && viewportPoint.y < 1f;
            if (!inViewport) return false;
            
            bool blocked = Physics.Linecast(_mainCamera.transform.position, transform.position, _obstacleLayer);
            
            return !blocked;
        }
    }
}