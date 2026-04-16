using _02.Scripts.Enemy;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private CutsceneEffectReceiver _effectReceiver;

    [Header("Look Target")]
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private Transform _cameraTarget;

    [Header("Letterbox")]
    [SerializeField] private RectTransform _topBar;
    [SerializeField] private RectTransform _bottomBar;
    [SerializeField] private float _letterboxHeight = 120f;
    [SerializeField] private float _letterboxIntroDuration = 1f;
    [SerializeField] private float _letterboxOutroDuration = 0.4f;

    [Header("HUD")]
    [SerializeField] private CanvasGroup _hudCanvasGroup;

    [Header("Camera Rotation")]
    [SerializeField] private float _lookRotationDuration = 2.5f;

    private bool _hasTriggered;
    private Sequence _cutsceneSeq;
    private Animator _hudAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        _hasTriggered = true;
        _playerController.EnterCutsceneMode();

        StartCutsceneIntro();
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

        _cutsceneSeq.AppendCallback(() => StartCoroutine(SmoothLookAt()));
    }

    private IEnumerator SmoothLookAt()
    {
        Transform player = _playerController.transform;
        Quaternion startRot = player.rotation;

        Vector3 dir = _lookTarget.position - player.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
        {
            dir = player.forward;
        }

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);

        if (_cameraTarget != null)
        {
            _cameraTarget.localRotation = Quaternion.identity;
        }

        float elapsed = 0f;
        while (elapsed < _lookRotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / _lookRotationDuration);
            player.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        player.rotation = targetRot;

        _director.stopped += OnDirectorStopped;
        _director.Play();
    }

    private void OnDirectorStopped(PlayableDirector director)
    {
        _director.stopped -= OnDirectorStopped;
        _effectReceiver.OnEnemyLookAtPlayer(EndCutsceneOutro);
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
            _effectReceiver.OnStartChaseAnimation();
            _playerController.ExitCutsceneMode();
            _enemyController.Activate();
        });
    }

    private void OnDestroy()
    {
        _cutsceneSeq?.Kill();
        _director.stopped -= OnDirectorStopped;
    }
}