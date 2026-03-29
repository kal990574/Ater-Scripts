using System;
using UnityEngine;

public class TimingQTERunner : IQuickTimeEvent
{
    private const float MaxProgress = 100f;

    private readonly TimingQuickTimeEventConfig _config;
    private readonly ITimingQuickTimeEventView _view;

    private float _notificationRemainingTime;
    private float _previousNeedleProgress;
    private float _needleProgress;
    private float _successZoneStartProgress;
    private float _successZoneSizeProgress;
    private float _greatZonePercent;

    public bool IsPlaying { get; private set; }
    public bool IsFinished { get; private set; }
    public EQuickTimeEventResult Result { get; private set; }

    public event Action<EQuickTimeEventResult> OnEnded;

    public TimingQTERunner(TimingQuickTimeEventConfig config, ITimingQuickTimeEventView view)
    {
        _config = config;
        _view = view;
    }

    public void Begin()
    {
        if (_config == null)
        {
            throw new InvalidOperationException($"[{nameof(TimingQTERunner)}] Config is missing.");
        }

        IsPlaying = true;
        IsFinished = false;
        Result = EQuickTimeEventResult.Default;
        _notificationRemainingTime = Mathf.Max(0f, _config.NotificationDuration);

        PlayNotification();

        if (_notificationRemainingTime <= 0f)
        {
            BeginJudgementPhase();
        }
    }

    public void Tick(float deltaTime)
    {
        if (IsPlaying == false)
        {
            return;
        }

        if (UpdateNotification(deltaTime))
        {
            return;
        }

        _previousNeedleProgress = _needleProgress;
        UpdateNeedle(deltaTime);
        UpdateView();
        CheckMissedSuccessZone();
    }

    public void Submit()
    {
        if (IsPlaying == false || _notificationRemainingTime > 0f)
        {
            return;
        }

        End(JudgeCurrentNeedleProgress());
    }

    public void Cancel()
    {
        if (IsPlaying == false && IsFinished)
        {
            return;
        }

        End(EQuickTimeEventResult.Fail);
    }

    private void UpdateNeedle(float deltaTime)
    {
        float delta = _config.NeedleSpeedPerSecond * deltaTime;

        if (_config.RotateClockwise)
        {
            _needleProgress += delta;
            return;
        }

        _needleProgress -= delta;
    }

    private void CheckMissedSuccessZone()
    {
        float successEndProgress = _successZoneStartProgress + _successZoneSizeProgress;
        bool hasPassedSuccessEnd = HasProgressPassed(_previousNeedleProgress, _needleProgress, successEndProgress);

        if (hasPassedSuccessEnd)
        {
            End(EQuickTimeEventResult.Fail);
        }
    }

    private bool UpdateNotification(float deltaTime)
    {
        if (_notificationRemainingTime <= 0f)
        {
            return false;
        }

        _notificationRemainingTime = Mathf.Max(0f, _notificationRemainingTime - deltaTime);
        if (_notificationRemainingTime > 0f)
        {
            return true;
        }

        BeginJudgementPhase();
        return false;
    }

    private void BeginJudgementPhase()
    {
        _previousNeedleProgress = 0f;
        _needleProgress = 0f;

        _successZoneSizeProgress = UnityEngine.Random.Range(_config.SuccessZoneSizeRange.x, _config.SuccessZoneSizeRange.y);
        _greatZonePercent = UnityEngine.Random.Range(_config.GreatZonePercentRange.x, _config.GreatZonePercentRange.y);

        float maxStartProgress = MaxProgress - _successZoneSizeProgress;
        _successZoneStartProgress = UnityEngine.Random.Range(0f, maxStartProgress);

        if (_view != null)
        {
            _view.Show();
            UpdateView();
        }
    }

    private void PlayNotification()
    {
        if (_config.NotificationClip == null)
        {
            return;
        }

        SoundManager soundManager = UnityEngine.Object.FindFirstObjectByType<SoundManager>();
        if (soundManager == null)
        {
            return;
        }

        soundManager.PlaySFX2D(_config.NotificationClip);
    }

    private void PlayResultFeedback(EQuickTimeEventResult result)
    {
        AudioClip clip = result switch
        {
            EQuickTimeEventResult.Success => _config.SuccessClip,
            EQuickTimeEventResult.GreatSuccess => _config.GreatSuccessClip,
            EQuickTimeEventResult.Fail => _config.FailClip,
            _ => null
        };

        if (clip == null)
        {
            return;
        }

        SoundManager soundManager = UnityEngine.Object.FindFirstObjectByType<SoundManager>();
        if (soundManager == null)
        {
            return;
        }

        soundManager.PlaySFX2D(clip);
    }

    private EQuickTimeEventResult JudgeCurrentNeedleProgress()
    {
        float currentProgress = GetNormalizedProgress(_needleProgress);
        float successStart = _successZoneStartProgress;
        float successEnd = _successZoneStartProgress + _successZoneSizeProgress;

        float greatSize = _successZoneSizeProgress * Mathf.Clamp01(_greatZonePercent / 100f);
        float greatStart = successStart;
        float greatEnd = successStart + greatSize;

        bool isInSuccess = IsProgressInRange(currentProgress, successStart, successEnd);
        bool isInGreat = IsProgressInRange(currentProgress, greatStart, greatEnd);

        if (isInGreat)
        {
            return EQuickTimeEventResult.GreatSuccess;
        }

        if (isInSuccess)
        {
            return EQuickTimeEventResult.Success;
        }

        return EQuickTimeEventResult.Fail;
    }

    private void End(EQuickTimeEventResult result)
    {
        if (IsPlaying == false && IsFinished)
        {
            return;
        }

        IsPlaying = false;
        IsFinished = true;
        Result = result;
        PlayResultFeedback(result);

        if (_view != null)
        {
            _view.ResetView();
            _view.Hide();
        }

        OnEnded?.Invoke(Result);
    }

    private void UpdateView()
    {
        if (_view == null)
        {
            return;
        }

        _view.UpdateView(
            _successZoneSizeProgress,
            _greatZonePercent,
            _successZoneStartProgress,
            GetNormalizedProgress(_needleProgress));
    }

    private float GetNormalizedProgress(float progress)
    {
        progress %= MaxProgress;
        if (progress < 0f)
        {
            progress += MaxProgress;
        }

        return progress;
    }

    private bool IsProgressInRange(float progress, float start, float end)
    {
        progress = GetNormalizedProgress(progress);
        start = GetNormalizedProgress(start);
        end = GetNormalizedProgress(end);

        if (start <= end)
        {
            return progress >= start && progress <= end;
        }

        return progress >= start || progress <= end;
    }

    private bool HasProgressPassed(float previousProgress, float currentProgress, float targetProgress)
    {
        float previousNormalized = GetNormalizedProgress(previousProgress);
        float currentNormalized = GetNormalizedProgress(currentProgress);
        float targetNormalized = GetNormalizedProgress(targetProgress);

        if (_config.RotateClockwise)
        {
            if (previousNormalized <= currentNormalized)
            {
                return previousNormalized < targetNormalized && targetNormalized <= currentNormalized;
            }

            return previousNormalized < targetNormalized || targetNormalized <= currentNormalized;
        }

        if (currentNormalized <= previousNormalized)
        {
            return currentNormalized <= targetNormalized && targetNormalized < previousNormalized;
        }

        return targetNormalized < previousNormalized || currentNormalized <= targetNormalized;
    }
}
