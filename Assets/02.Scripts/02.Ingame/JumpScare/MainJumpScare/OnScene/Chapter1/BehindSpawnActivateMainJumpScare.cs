using Sirenix.OdinInspector;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BehindSpawnActivateMainJumpScare : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject _targetObject;

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnDistance = 2.0f;
    [SerializeField] private float _heightOffset = 0.0f;
    [SerializeField] private bool _lookAtPlayer = true;
    [SerializeField] private bool _usePlayerYawOnly = true;

    [Button]
    public void Execute()
    {
        if (_playerTransform == null)
        {
            Debug.LogWarning($"{nameof(BehindSpawnActivateMainJumpScare)}: Player Transform reference is missing.", this);
            return;
        }

        if (_targetObject == null)
        {
            Debug.LogWarning($"{nameof(BehindSpawnActivateMainJumpScare)}: Target Object reference is missing.", this);
            return;
        }

        _targetObject.SetActive(true);
        
        Transform targetTransform = _targetObject.transform;

        Vector3 backward = GetBackwardDirection();
        Vector3 spawnPosition = _playerTransform.position + backward * _spawnDistance;
        spawnPosition.y += _heightOffset;

        targetTransform.position = spawnPosition;

        if (_lookAtPlayer == true)
        {
            RotateTargetToPlayer(targetTransform, spawnPosition);
        }

        if (_targetObject.activeSelf == false)
        {
            _targetObject.SetActive(true);
        }
    }

    private Vector3 GetBackwardDirection()
    {
        if (_usePlayerYawOnly == true)
        {
            Vector3 backward = -_playerTransform.forward;
            backward.y = 0.0f;

            if (backward.sqrMagnitude <= 0.0001f)
            {
                return Vector3.back;
            }

            return backward.normalized;
        }

        return (-_playerTransform.forward).normalized;
    }

    private void RotateTargetToPlayer(Transform targetTransform, Vector3 spawnPosition)
    {
        Vector3 lookDirection = _playerTransform.position - spawnPosition;

        if (_usePlayerYawOnly == true)
        {
            lookDirection.y = 0.0f;
        }

        if (lookDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        targetTransform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
    }
}
