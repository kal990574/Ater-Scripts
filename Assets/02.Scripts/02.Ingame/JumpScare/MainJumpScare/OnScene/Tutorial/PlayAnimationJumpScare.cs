using Sirenix.OdinInspector;
using UnityEngine;

public class PlayAnimationJumpScare : MainJumpScareBase
{
    [Header("References")]
    [SerializeField] private Animator _targetAnimator;
    [SerializeField] private SoundSFXEmitter _soundEmitter;

    [Header("Animation")]
    [SerializeField] private string _triggerParameterName = "Open";
    [SerializeField] private bool _resetTriggerBeforeSet = true;

    [Header("Default State")]
    [SerializeField] private bool _playOnExecuteOnlyOnce = true;

    [Header("Debug")]
    [SerializeField] private bool _enableLog = true;

    [Header("Runtime")]
    [SerializeField, ReadOnly] private bool _isPlayingAnimation;
    [SerializeField, ReadOnly] private bool _hasFinishedAnimation;

    private int _triggerParameterHash;

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

        PlaySound();
        PlayAnimation();

        if (_enableLog == true)
        {
            Debug.Log($"[{name}] 애니메이션 메인 점프스케어를 실행했습니다.", this);
        }
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

    private void PlaySound()
    {
        if (_soundEmitter == null)
        {
            if (_enableLog == true)
            {
                Debug.LogWarning($"[{name}] SoundSFXEmitter 가 없어 사운드를 재생하지 않습니다.", this);
            }

            return;
        }

        _soundEmitter.Play();
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
}