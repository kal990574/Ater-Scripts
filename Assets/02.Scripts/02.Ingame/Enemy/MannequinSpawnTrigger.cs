using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace _02.Scripts.Enemy
{
    public class MannequinSpawnTrigger : MonoBehaviour
    {
        [Header("Wall")]
        [SerializeField] private Transform _wallTransform;
        [SerializeField] private float _wallRiseHeight = 3f;
        [SerializeField] private float _wallRiseDuration = 0.3f;
        [SerializeField] private Ease _wallRiseEase = Ease.OutQuad;

        [Header("Camera Shake")]
        [SerializeField] private CinemachineImpulseSource _impulseSource;
        [SerializeField] private float _impulseForce = 1.5f;

        [Header("Mannequins")]
        [SerializeField] private EnemyController[] _mannequins;
        [SerializeField] private float _activationDelay = 0.2f;

        [Header("Events")]
        [SerializeField] private UnityEvent _onWallRise;
        [SerializeField] private UnityEvent _onMannequinsActivated;

        [Header("Detection")]
        [SerializeField] private LayerMask _playerLayer;

        private bool _triggered;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered) return;
            if (((1 << other.gameObject.layer) & _playerLayer) == 0) return;

            _triggered = true;
            StartCoroutine(SpawnSequence());
        }

        private IEnumerator SpawnSequence()
        {
            float targetY = _wallTransform.localPosition.y + _wallRiseHeight;
            _wallTransform.DOLocalMoveY(targetY, _wallRiseDuration).SetEase(_wallRiseEase);

            if (_impulseSource != null)
                _impulseSource.GenerateImpulseWithForce(_impulseForce);

            _onWallRise?.Invoke();

            yield return new WaitForSeconds(_activationDelay);

            foreach (var mannequin in _mannequins)
            {
                if (mannequin != null)
                    mannequin.Activate();
            }

            _onMannequinsActivated?.Invoke();
        }

        private void OnDestroy()
        {
            _wallTransform.DOKill();
        }
    }
}