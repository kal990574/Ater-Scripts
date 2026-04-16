using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;
using System;

public class CutsceneEffectReceiver : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _doorImpulseForce = 2.0f;

    [Header("Enemy")]
    [SerializeField] private NavMeshAgent _enemyAgent;
    [SerializeField] private Animator _enemyAnimator;
    [SerializeField] private RuntimeAnimatorController _chaseAnimatorController;
    [SerializeField] private Transform _enemyHead;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _headTurnDuration = 0.5f;

    public void OnDisableEnemyAgent()
    {
        if (_enemyAgent != null) _enemyAgent.enabled = false;
    }

    public void OnStartChaseAnimation()
    {
        _enemyAnimator.runtimeAnimatorController = _chaseAnimatorController;
    }

    public void OnDoorImpulse()
    {
        _impulseSource.GenerateImpulseWithForce(_doorImpulseForce);
    }

    private bool _isHeadTurning;
    private Quaternion _headTargetRot;
    private Quaternion _headStartRot;
    private float _headTurnElapsed;
    private Action _onHeadTurnComplete;

    public void OnEnemyLookAtPlayer(Action onComplete = null)
    {
        _headStartRot = _enemyHead.rotation;

        Vector3 dir = _playerTransform.position - _enemyHead.position;
        dir.y -= 4f;
        dir.x += 2f;
        _headTargetRot = Quaternion.LookRotation(dir.normalized);

        _headTurnElapsed = 0f;
        _isHeadTurning = true;
        _onHeadTurnComplete = onComplete;
    }

    private void LateUpdate()
    {
        if (!_isHeadTurning) return;

        _headTurnElapsed += Time.deltaTime;
        float t = Mathf.SmoothStep(0f, 1f, _headTurnElapsed / _headTurnDuration);
        _enemyHead.rotation = Quaternion.Slerp(_headStartRot, _headTargetRot, t);

        if (_headTurnElapsed >= _headTurnDuration)
        {
            _isHeadTurning = false;
            _onHeadTurnComplete?.Invoke();
            _onHeadTurnComplete = null;
        }
    }
}