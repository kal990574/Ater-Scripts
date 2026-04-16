using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Unity.Cinemachine;
using System.Collections;
using System;

public class CutsceneEffectReceiver : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _doorImpulseForce = 2.0f;

    [Header("Post Processing")]
    [SerializeField] private Volume _cutsceneVolume;
    [SerializeField] private float _effectDuration = 1.5f;

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

    public void OnEnemyReveal()
    {
        if (_cutsceneVolume == null) return;
        StartCoroutine(EnemyRevealEffect());
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

    private IEnumerator EnemyRevealEffect()
    {
        _cutsceneVolume.weight = 1f;

        float elapsed = 0f;
        while (elapsed < _effectDuration)
        {
            elapsed += Time.deltaTime;
            _cutsceneVolume.weight = Mathf.Lerp(1f, 0f, elapsed / _effectDuration);
            yield return null;
        }

        _cutsceneVolume.weight = 0f;
    }
}