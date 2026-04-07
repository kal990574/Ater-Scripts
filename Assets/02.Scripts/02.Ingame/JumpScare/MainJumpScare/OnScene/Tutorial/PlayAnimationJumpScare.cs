using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class PlayAnimationJumpScare : MainJumpScareBase
{
    private const int DefaultAnimatorLayer = 0;

    [Header("References")]
    [SerializeField] private Animator _targetAnimator;

    [Header("Animation")]
    [SerializeField] private string _triggerParameterName = "Open";
    [SerializeField] private bool _resetTriggerBeforeSet = true;

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

    private int _triggerParameterHash;
    private Coroutine _monitorCoroutine;

    protected override void Awake()
    {
        base.Awake();

        _triggerParameterHash = Animator.StringToHash(_triggerParameterName);
        InitializeRuntimeState();
    }

    private void Reset()
    {
        _targetAnimator = GetComponentInChildren<Animator>();
    }

    [Button]
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
        if (string.IsNullOrWhiteSpace(_triggerParameterName) == true)
        {
            Debug.LogError($"[{name}] 트리거 파라미터명이 비어 있습니다.", this);
            FinishAnimationJumpScare();
            return;
        }

        if (_resetTriggerBeforeSet == true)
        {
            _targetAnimator.ResetTrigger(_triggerParameterHash);
        }

        _targetAnimator.SetTrigger(_triggerParameterHash);
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
        AnimatorStateInfo initialState = _targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
        int initialFullPathHash = initialState.fullPathHash;

        float elapsed = 0f;
        bool hasEnteredTriggeredState = false;
        int playingStateHash = 0;

        while (_isPlayingAnimation == true && elapsed < _animationStartTimeout)
        {
            if (_targetAnimator.IsInTransition(layerIndex) == true)
            {
                hasEnteredTriggeredState = true;
            }

            AnimatorStateInfo currentState = _targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
            if (currentState.fullPathHash != initialFullPathHash)
            {
                hasEnteredTriggeredState = true;
                playingStateHash = currentState.fullPathHash;
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

        if (hasEnteredTriggeredState == false)
        {
            if (_enableLog == true)
            {
                Debug.LogWarning($"[{name}] 트리거된 애니메이션 상태 진입을 확인하지 못해 강제로 종료합니다.", this);
            }

            NotifyAnimationFinished();
            _monitorCoroutine = null;
            yield break;
        }

        if (playingStateHash == 0)
        {
            AnimatorStateInfo currentState = _targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
            playingStateHash = currentState.fullPathHash;
        }

        while (_isPlayingAnimation == true)
        {
            if (_targetAnimator.IsInTransition(layerIndex) == true)
            {
                AnimatorStateInfo nextState = _targetAnimator.GetNextAnimatorStateInfo(layerIndex);
                if (nextState.fullPathHash != 0 && nextState.fullPathHash != playingStateHash)
                {
                    break;
                }

                yield return null;
                continue;
            }

            AnimatorStateInfo currentState = _targetAnimator.GetCurrentAnimatorStateInfo(layerIndex);
            if (currentState.fullPathHash != playingStateHash)
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

    [Button]
    public void ResetJumpScare()
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
