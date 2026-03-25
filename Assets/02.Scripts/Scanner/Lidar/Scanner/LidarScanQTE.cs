using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LidarScanQTE : IQTEInvoker
{
    private readonly LidarScanQTESettings _settings;

    private ScannableObject _currentTarget;
    private float _elapsedTime;
    private float _nextTriggerTime;
    private bool _isQteActive;

    public LidarScanQTE(LidarScanQTESettings settings)
    {
        _settings = settings;
        ResetTriggerTimer();
    }

    //QTE 도중 타겟변경 감지
    public bool HandleTargetChanged(ScannableObject previousTarget, ScannableObject newTarget)
    {
        if (ReferenceEquals(previousTarget, newTarget))
        {
            _currentTarget = newTarget;
            return true;
        }

        bool shouldNotifyScanLost = true;
        if (_isQteActive && previousTarget != null && QTEManager.Instance != null)
        {
            //QTE중 바뀌엇다면 기존 QTE 강제 패배
            shouldNotifyScanLost = false;
            QTEManager.Instance.ForceFailByOwner(this);
        }
        
        
        //새로운 타겟에 대해 새로운 딜레이
        _currentTarget = newTarget;
        ResetTriggerTimer();
        return shouldNotifyScanLost;
    }

    public bool HandleStop(ScannableObject currentTarget)
    {
        if (_isQteActive == false || currentTarget == null || ReferenceEquals(_currentTarget, currentTarget) == false)
        {
            _currentTarget = null;
            ResetTriggerTimer();
            return true;
        }

        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.ForceFailByOwner(this);
        }

        _currentTarget = null;
        ResetTriggerTimer();
        return false;
    }
    
    //현재 타겟의 QTE 시간 업데이트 및 시간이 된 경우 QTE 발동
    public void UpdateCurrentTarget(ScannableObject target, float deltaTime)
    {
        if (target == null || _settings == null || _isQteActive)
        {
            return;
        }

        if (target.State != EScannableState.OnProgress)
        {
            return;
        }

        _elapsedTime += deltaTime;
        if (_elapsedTime < _nextTriggerTime)
        {
            return;
        }

        TryStartQte(target);
    }

    //QTE에 입력이 들어온경우
    public void SubmitCurrent()
    {
        if (QTEManager.Instance == null)
        {
            return;
        }

        QTEManager.Instance.SubmitCurrent();
    }
    
    //QTE 실패
    public void ApplyQTEFailure()
    {
        if (_currentTarget == null)
        {
            return;
        }

        _currentTarget.ReduceProgress(_settings.FailPenalty);
        _currentTarget.ChangeState(_currentTarget.CurrentProgress <= 0.0f ? EScannableState.Default : EScannableState.OnReturn);
    }
    
    //QTE 성공 반영
    public void ApplyQTESuccess()
    {
        if (_currentTarget == null)
        {
            return;
        }

        _currentTarget.ChangeState(_currentTarget.IsProgressComplete ? EScannableState.OnCompleted : EScannableState.OnProgress);
    }

    //QTE 대성공 반영
    public void ApplyQTEGreatSuccess()
    {
        if (_currentTarget == null)
        {
            return;
        }

        _currentTarget.AddProgress(_settings.GreatSuccessBonus);
        if (_currentTarget.TryTransitToCompleted())
        {
            return;
        }

        _currentTarget.ChangeState(EScannableState.OnProgress);
    }

    //QTE가 가능한지 체크후 가능하다면 플레이
    private void TryStartQte(ScannableObject target)
    {
        if (QTEManager.Instance == null)
        {
            return;
        }

        _currentTarget = target;
        bool started = QTEManager.Instance.TryPlay(this, HandleQteEnded);
        if (started == false)
        {
            return;
        }

        _isQteActive = true;
        _currentTarget.PauseScanning();
    }

    private void HandleQteEnded(EQuickTimeEventResult result)
    {
        _isQteActive = false;

        switch (result)
        {
            case EQuickTimeEventResult.Fail:
                ApplyQTEFailure();
                break;
            case EQuickTimeEventResult.Success:
                ApplyQTESuccess();
                break;
            case EQuickTimeEventResult.GreatSuccess:
                ApplyQTEGreatSuccess();
                break;
            default:
                if (_currentTarget != null)
                {
                    _currentTarget.ChangeState(EScannableState.OnProgress);
                }
                break;
        }

        ResetTriggerTimer();
    }

    private void ResetTriggerTimer()
    {
        _elapsedTime = 0.0f;
        if (_settings == null)
        {
            _nextTriggerTime = float.MaxValue;
            return;
        }

        _nextTriggerTime = Random.Range(_settings.MinInterval, _settings.MaxInterval);
    }
}
