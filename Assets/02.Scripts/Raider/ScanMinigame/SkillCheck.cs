using System;
using UnityEngine;

public class MinigameExample : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private MinigameUI _ui;

    [Header("Input")]
    [SerializeField] private KeyCode _inputKey = KeyCode.Space;

    [Header("Needle")]
    [SerializeField] private float _needleSpeedPerSecond = 100f;
    [SerializeField] private bool _rotateClockwise = true;

    [Header("Zone Range (0~100 Progress)")]
    [SerializeField] private Vector2 _successZoneSizeRange = new Vector2(12.5f, 25f);
    [SerializeField] private Vector2 _greatZonePercentRange = new Vector2(15f, 35f);

    [Header("Start")]
    [SerializeField] private bool _playOnEnable = false;
    [SerializeField] private bool _hideOnEnd = true;

    private const float MaxProgress = 100f;

    private bool _isPlaying;

    private float _previousNeedleProgress;
    private float _needleProgress;
    private float _successZoneStartProgress;
    private float _successZoneSizeProgress;
    private float _greatZonePercent;

    public bool IsPlaying
    {
        get
        {
            return _isPlaying;
        }
    }

    public event Action<EMinigameResult> OnSkillCheckEnded;
    public event Action OnGreat;
    public event Action OnSuccess;
    public event Action OnFail;
    public event Action OnTimeout;

    private void OnEnable()
    {
        if (_playOnEnable == true)
        {
            StartSkillCheck();
        }
        else
        {
            if (_ui != null)
            {
                _ui.ResetAll();
                _ui.Show(false);
            }
        }
    }

    private void Update()
    {
        if (_isPlaying == false)
        {
            return;
        }

        _previousNeedleProgress = _needleProgress;

        UpdateNeedle();
        UpdateUI();
        HandleInput();
        CheckMissedSuccessZone();
    }

    public void StartSkillCheck()
    {
        _isPlaying = true;

        _previousNeedleProgress = 0f;
        _needleProgress = 0f;

        _successZoneSizeProgress = UnityEngine.Random.Range(_successZoneSizeRange.x, _successZoneSizeRange.y);
        _greatZonePercent = UnityEngine.Random.Range(_greatZonePercentRange.x, _greatZonePercentRange.y);

        float maxStartProgress = MaxProgress - _successZoneSizeProgress;
        _successZoneStartProgress = UnityEngine.Random.Range(0f, maxStartProgress);

        if (_ui != null)
        {
            _ui.Show(true);
            UpdateUI();
        }
    }

    public void CancelSkillCheck()
    {
        _isPlaying = false;

        if (_ui != null)
        {
            _ui.ResetAll();

            if (_hideOnEnd == true)
            {
                _ui.Show(false);
            }
        }
    }

    private void UpdateNeedle()
    {
        float delta = _needleSpeedPerSecond * Time.deltaTime;

        if (_rotateClockwise == true)
        {
            _needleProgress += delta;
        }
        else
        {
            _needleProgress -= delta;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(_inputKey) == false)
        {
            return;
        }

        EMinigameResult result = JudgeCurrentNeedleProgress();
        EndSkillCheck(result);
    }

    private void CheckMissedSuccessZone()
    {
        if (_isPlaying == false)
        {
            return;
        }

        float successEndProgress = _successZoneStartProgress + _successZoneSizeProgress;
        bool hasPassedSuccessEnd = HasProgressPassed(_previousNeedleProgress, _needleProgress, successEndProgress);

        if (hasPassedSuccessEnd == true)
        {
            EndSkillCheck(EMinigameResult.Fail);
        }
    }

    private EMinigameResult JudgeCurrentNeedleProgress()
    {
        float currentProgress = GetNormalizedProgress(_needleProgress);

        float successStart = _successZoneStartProgress;
        float successEnd = _successZoneStartProgress + _successZoneSizeProgress;

        float greatSize = _successZoneSizeProgress * Mathf.Clamp01(_greatZonePercent / 100f);
        float greatStart = successStart;
        float greatEnd = successStart + greatSize;

        bool isInSuccess = IsProgressInRange(currentProgress, successStart, successEnd);
        bool isInGreat = IsProgressInRange(currentProgress, greatStart, greatEnd);

        if (isInGreat == true)
        {
            return EMinigameResult.GreatSuccess;
        }

        if (isInSuccess == true)
        {
            return EMinigameResult.Success;
        }

        return EMinigameResult.Fail;
    }

    private void EndSkillCheck(EMinigameResult result)
    {
        if (_isPlaying == false)
        {
            return;
        }

        _isPlaying = false;

        switch (result)
        {
            case EMinigameResult.GreatSuccess:
            {
                OnGreat?.Invoke();
                break;
            }
            case EMinigameResult.Success:
            {
                OnSuccess?.Invoke();
                break;
            }
            case EMinigameResult.Fail:
            {
                OnFail?.Invoke();
                break;
            }
        }

        OnSkillCheckEnded?.Invoke(result);

        if (_ui != null)
        {
            _ui.ResetAll();

            if (_hideOnEnd == true)
            {
                _ui.Show(false);
            }
        }
    }

    private void UpdateUI()
    {
        if (_ui == null)
        {
            return;
        }

        _ui.SetSkillCheckUI(
            _successZoneSizeProgress,
            _greatZonePercent,
            _successZoneStartProgress,
            GetNormalizedProgress(_needleProgress)
        );
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

        if (_rotateClockwise == true)
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