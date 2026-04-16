using _02.Scripts.Enemy;
using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class BarrierCutsceneController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _cameraTarget;

    [Header("Look Target")]
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private float _lookRotationDuration = 1.5f;

    [Header("Letterbox")]
    [SerializeField] private RectTransform _topBar;
    [SerializeField] private RectTransform _bottomBar;
    [SerializeField] private float _letterboxHeight = 120f;
    [SerializeField] private float _letterboxIntroDuration = 0.8f;
    [SerializeField] private float _letterboxOutroDuration = 0.4f;
    [SerializeField] private CanvasGroup _hudCanvasGroup;

    [Header("Enemies - Deactivate")]
    [SerializeField] private EnemyController[] _chaseEnemies;

    [Header("Enemies - Cinematic")]
    [SerializeField] private GameObject[] _cinematicEnemies;

    [Header("Barrier")]
    [SerializeField] private Transform _barrierTransform;
    [SerializeField] private float _barrierDropHeight = 3f;
    [SerializeField] private float _barrierDropDuration = 0.2f;
    [SerializeField] private Ease _barrierDropEase = Ease.InQuad;

    [Header("Camera Shake")]
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _impulseForce = 2f;

    [Header("Light")]
    [SerializeField] private Light[] _spotLights;
    [SerializeField] private float _targetRange = 20f;
    [SerializeField] private float _targetIntensity = 3f;
    [SerializeField] private float _lightFadeDuration = 0.5f;

    [Header("Timing")]
    [SerializeField] private float _enemyShowDuration = 2f;
    [SerializeField] private float _barrierDropDelay = 0f;
    [SerializeField] private float _outroDelay = 0.5f;

    [Header("Events")]
    [SerializeField] private UnityEvent _onBarrierDrop;
    [SerializeField] private UnityEvent _onCutsceneEnd;

    private Sequence _cutsceneSeq;
    private Animator _hudAnimator;

    public void Play()
    {
        DeactivateChaseEnemies();
        _playerController.EnterCutsceneMode();
        StartCutsceneIntro();
    }

    private void DeactivateChaseEnemies()
    {
        foreach (var enemy in _chaseEnemies)
        {
            if (enemy != null && enemy.gameObject.activeSelf)
                enemy.ForceDeactivate();
        }
    }

    private void StartCutsceneIntro()
    {
        _hudAnimator = _hudCanvasGroup.GetComponent<Animator>();
        if (_hudAnimator != null) _hudAnimator.enabled = false;

        _cutsceneSeq = DOTween.Sequence();

        _cutsceneSeq.Append(
            _topBar.DOSizeDelta(new Vector2(_topBar.sizeDelta.x, _letterboxHeight), _letterboxIntroDuration)
                .SetEase(Ease.OutQuad)
        );
        _cutsceneSeq.Join(
            _bottomBar.DOSizeDelta(new Vector2(_bottomBar.sizeDelta.x, _letterboxHeight), _letterboxIntroDuration)
                .SetEase(Ease.OutQuad)
        );
        _cutsceneSeq.Join(
            _hudCanvasGroup.DOFade(0f, _letterboxIntroDuration)
        );

        _cutsceneSeq.AppendCallback(() => StartCoroutine(CutsceneSequence()));
    }

    private IEnumerator CutsceneSequence()
    {
        yield return StartCoroutine(SmoothLookAt());

        foreach (var light in _spotLights)
        {
            if (light == null) continue;
            DOTween.To(() => light.range, x => light.range = x, (float)_targetRange, _lightFadeDuration);
            DOTween.To(() => light.intensity, x => light.intensity = x, (float)_targetIntensity, _lightFadeDuration);
        }

        foreach (var enemy in _cinematicEnemies)
        {
            if (enemy != null)
                enemy.SetActive(true);
        }

        yield return new WaitForSeconds(_enemyShowDuration);

        if (_barrierDropDelay > 0f)
            yield return new WaitForSeconds(_barrierDropDelay);

        DropBarrier();

        yield return new WaitForSeconds(_barrierDropDuration);

        DeactivateCinematicEnemies();

        if (_outroDelay > 0f)
            yield return new WaitForSeconds(_outroDelay);

        EndCutsceneOutro();
    }

    private IEnumerator SmoothLookAt()
    {
        Transform player = _playerController.transform;
        Quaternion startRot = player.rotation;

        Vector3 dir = _lookTarget.position - player.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            dir = player.forward;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);

        if (_cameraTarget != null)
            _cameraTarget.localRotation = Quaternion.identity;

        float elapsed = 0f;
        while (elapsed < _lookRotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / _lookRotationDuration);
            player.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        player.rotation = targetRot;
    }

    private void DeactivateCinematicEnemies()
    {
        foreach (var enemy in _cinematicEnemies)
        {
            if (enemy != null)
                enemy.SetActive(false);
        }
    }

    private void DropBarrier()
    {
        float targetY = _barrierTransform.localPosition.y - _barrierDropHeight;
        _barrierTransform.DOLocalMoveY(targetY, _barrierDropDuration)
            .SetEase(_barrierDropEase)
            .OnComplete(() =>
            {
                if (_impulseSource != null)
                    _impulseSource.GenerateImpulseWithForce(_impulseForce);

                _onBarrierDrop?.Invoke();
            });
    }

    private void EndCutsceneOutro()
    {
        _cutsceneSeq?.Kill();
        _cutsceneSeq = DOTween.Sequence();

        _cutsceneSeq.Append(
            _topBar.DOSizeDelta(new Vector2(_topBar.sizeDelta.x, 0f), _letterboxOutroDuration)
                .SetEase(Ease.InQuad)
        );
        _cutsceneSeq.Join(
            _bottomBar.DOSizeDelta(new Vector2(_bottomBar.sizeDelta.x, 0f), _letterboxOutroDuration)
                .SetEase(Ease.InQuad)
        );
        _cutsceneSeq.Join(
            _hudCanvasGroup.DOFade(1f, _letterboxOutroDuration)
        );

        _cutsceneSeq.OnComplete(() =>
        {
            if (_hudAnimator != null) _hudAnimator.enabled = true;
            _playerController.ExitCutsceneMode();
            _onCutsceneEnd?.Invoke();
        });
    }

    private void OnDestroy()
    {
        _cutsceneSeq?.Kill();
        _barrierTransform.DOKill();
    }
}