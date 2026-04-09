using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class PlayAnimationJumpScare : MainJumpScareBase
{
    private const int DefaultAnimatorLayer = 0;

    [Header("References")]
    [SerializeField] private Animator _targetAnimator;

    [Header("Animation")]
    [SerializeField] private string _animationStateName = "OpenDoor";
    [SerializeField] private float _normalizedStartTime = 0.0f;

    [Header("Default State")]
    [SerializeField] private bool _playOnExecuteOnlyOnce = true;

    [Header("Detection")]
    [SerializeField] private int _monitorLayerIndex = DefaultAnimatorLayer;
    [SerializeField] private float _animationStartTimeout = 2f;

    [Header("Debug")]
    [SerializeField] private bool _enableLog = true;

    [Header("Runtime")]
    [SerializeField, ReadOnly] private bool _isPlayingAnimation;
    [SerializeField, ReadOnly] private bool _hasFinishedAnimation;

    private int _animationStateHash;
    private Coroutine _monitorCoroutine;

    protected override void Awake()
    {
        base.Awake();

        ResolveAnimatorReference();
        _animationStateHash = Animator.StringToHash(_animationStateName);
        InitializeRuntimeState();
    }

    private void Reset()
    {
        ResolveAnimatorReference();
    }
    
    protected override void OnExecute()
    {
        if (_targetAnimator == null)
        {
            Debug.LogError($"[{name}] Target Animator 가 없어 애니메이션 점프스케어를 실행할 수 없습니다.", this);
            NotifyFinished();
            return;
        }

        _isPlayingAnimation = true;
        _hasFinishedAnimation = false;
        
        PlayAnimation();

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 애니메이션 메인 점프스케어를 실행했습니다.", this);
        }

        StartAnimationMonitor();
    }

    private void PlayAnimation()
    {
        if (string.IsNullOrWhiteSpace(_animationStateName) == true)
        {
            Debug.LogError($"[{name}] 애니메이션 상태 이름이 비어 있습니다.", this);
            FinishAnimationJumpScare();
            return;
        }

        _targetAnimator.Play(_animationStateHash, GetValidLayerIndex(), _normalizedStartTime);
        _targetAnimator.Update(0.0f);
    }

    private void StartAnimationMonitor()
    {
        if (_monitorCoroutine != null)
        {
            StopCoroutine(_monitorCoroutine);
        }

        _monitorCoroutine = StartCoroutine(MonitorAnimationEnd());
    }

    private IEnumerator MonitorAnimationEnd()
    {
        int layerIndex = GetValidLayerIndex();
        float elapsed = 0f;
        bool hasEnteredTargetState = false;

        while (_isPlayingAnimation == true && elapsed < _animationStartTimeout)
        {
            AnimatorStateInfo currentState = _targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
            if (currentState.shortNameHash == _animationStateHash || currentState.fullPathHash == _animationStateHash)
            {
                hasEnteredTargetState = true;
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_isPlayingAnimation == false)
        {
            _monitorCoroutine = null;
            yield break;
        }

        if (hasEnteredTargetState == false)
        {
            if (_enableLog == true)
            {
                Debug.LogWarning($"[{name}] 대상 애니메이션 상태 진입을 확인하지 못해 강제로 종료합니다.", this);
            }

            NotifyAnimationFinished();
            _monitorCoroutine = null;
            yield break;
        }

        while (_isPlayingAnimation == true)
        {
            if (_targetAnimator.IsInTransition(layerIndex) == true)
            {
                yield return null;
                continue;
            }

            AnimatorStateInfo currentState = _targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
            bool isTargetState =
                currentState.shortNameHash == _animationStateHash || currentState.fullPathHash == _animationStateHash;

            if (isTargetState == false)
            {
                break;
            }

            if (currentState.loop == false && currentState.normalizedTime >= 1f)
            {
                break;
            }

            yield return null;
        }

        NotifyAnimationFinished();
        _monitorCoroutine = null;
    }
    
    public void NotifyAnimationFinished()
    {
        if (_isPlayingAnimation == false)
        {
            if (_enableLog == true)
            {
                Debug.LogWarning($"[{name}] 애니메이션 재생 중이 아닌데 종료 이벤트를 받았습니다.", this);
            }

            return;
        }

        if (_hasFinishedAnimation == true)
        {
            return;
        }

        _hasFinishedAnimation = true;
        FinishAnimationJumpScare();
    }

    protected override void OnResetJumpScare()
    {
        InitializeRuntimeState();

        if (_monitorCoroutine != null)
        {
            StopCoroutine(_monitorCoroutine);
            _monitorCoroutine = null;
        }

        if (_targetAnimator != null)
        {
            _targetAnimator.Rebind();
            _targetAnimator.Update(0.0f);
        }

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 애니메이션 메인 점프스케어를 리셋했습니다.", this);
        }
    }

    private void FinishAnimationJumpScare()
    {
        if (_isPlayingAnimation == false)
        {
            return;
        }

        _isPlayingAnimation = false;

        if (_monitorCoroutine != null)
        {
            StopCoroutine(_monitorCoroutine);
            _monitorCoroutine = null;
        }

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 애니메이션 메인 점프스케어를 종료합니다.", this);
        }

        NotifyFinished();
    }

    private void InitializeRuntimeState()
    {
        _isPlayingAnimation = false;
        _hasFinishedAnimation = false;
    }

    private void ResolveAnimatorReference()
    {
        if (_targetAnimator != null)
        {
            return;
        }

        _targetAnimator = transform.GetComponentInChildren<Animator>();
    }

    private int GetValidLayerIndex()
    {
        if (_targetAnimator == null)
        {
            return DefaultAnimatorLayer;
        }

        if (_monitorLayerIndex < 0 || _monitorLayerIndex >= _targetAnimator.layerCount)
        {
            if (_enableLog == true)
            {
                Debug.LogWarning(
                    $"[{name}] Monitor Layer Index({_monitorLayerIndex}) 가 유효하지 않아 기본 레이어(0)를 사용합니다.",
                    this);
            }

            return DefaultAnimatorLayer;
        }

        return _monitorLayerIndex;
    }
}
