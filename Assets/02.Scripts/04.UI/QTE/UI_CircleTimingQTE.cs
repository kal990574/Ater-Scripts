using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_CircleTimingQTE : MonoBehaviour, ITimingQuickTimeEventView
{
    private const float MaxProgress = 100f;
    private const float FullCircleAngle = 360f;

    [Header("Required References")]
    [SerializeField] private Image _successZoneImage;
    [SerializeField] private Image _greatZoneImage;
    [SerializeField] private Image _resultHitImage;
    [SerializeField] private Transform _judgeZoneTransform;
    [SerializeField] private Transform _needleTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _feedbackShakeTarget;

    [Header("Result Feedback")]
    [SerializeField] private float _hideDelayAfterSubmit = 1.0f;
    [SerializeField] private Color _successHitColor = new Color(0.6f, 1.0f, 0.4f, 1.0f);
    [SerializeField] private Color _greatSuccessHitColor = Color.green;
    [SerializeField] private Color _failHitColor = Color.red;
    [SerializeField] private float _hitColorDuration = 0.15f;
    [SerializeField] private float _hitFadeDuration = 0.85f;
    [SerializeField] private Ease _hitEase = Ease.OutQuad;

    [Header("Fail Shake")]
    [SerializeField] private float _failShakeDuration = 0.35f;
    [SerializeField] private float _failShakeStrength = 24.0f;
    [SerializeField] private int _failShakeVibrato = 8;
    [SerializeField] private float _failShakeElasticity = 0.8f;

    private Color _originalResultHitColor = Color.white;
    private Vector2 _defaultShakeAnchorPosition;
    private Tween _hideDelayTween;
    private Tween _resultHitTween;
    private Tween _failShakeTween;

    private void Awake()
    {
        if (_feedbackShakeTarget == null)
        {
            _feedbackShakeTarget = transform as RectTransform;
        }

        if (_feedbackShakeTarget != null)
        {
            _defaultShakeAnchorPosition = _feedbackShakeTarget.anchoredPosition;
        }

        if (_resultHitImage != null)
        {
            _originalResultHitColor = _resultHitImage.color;
            SetResultHitAlpha(0f);
        }
    }

    private void OnDestroy()
    {
        KillTweens();
    }

    public void Show()
    {
        KillTweens();
        ResetShakeTargetPosition();

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        SetResultHitAlpha(0f);
    }

    public void Hide()
    {
        KillTweens();
        ResetShakeTargetPosition();

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void ResetView()
    {
        UpdateView(0f, 0f, 0f, 0f);
        SetResultHitAlpha(0f);
        ResetShakeTargetPosition();
    }

    public void UpdateView(float successZoneSizeProgress, float greatZonePercent, float judgeZoneStartProgress, float needleProgress)
    {
        ChangeSuccessSize(successZoneSizeProgress);
        ChangeGreatRate(greatZonePercent);
        SetJudgeZone(judgeZoneStartProgress);
        RotateNeedle(needleProgress);
    }

    public void PlayResultFeedback(EQuickTimeEventResult result)
    {
        PlayResultHit(result);
        PlayFailShakeIfNeeded(result);

        _hideDelayTween?.Kill();
        _hideDelayTween = DOVirtual.DelayedCall(_hideDelayAfterSubmit, HideAfterFeedback)
            .SetLink(gameObject);
    }

    private void ChangeSuccessSize(float progress)
    {
        float normalized = Mathf.Clamp01(progress / MaxProgress);
        _successZoneImage.fillAmount = normalized;
    }

    private void ChangeGreatRate(float percent)
    {
        float normalized = Mathf.Clamp01(percent / 100f);
        _greatZoneImage.fillAmount = _successZoneImage.fillAmount * normalized;
    }

    private void SetJudgeZone(float startProgress)
    {
        float angle = ConvertProgressToAngle(startProgress);
        _judgeZoneTransform.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    private void RotateNeedle(float progress)
    {
        float angle = ConvertProgressToAngle(progress);
        _needleTransform.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    private float ConvertProgressToAngle(float progress)
    {
        float normalized = Mathf.Clamp01(progress / MaxProgress);
        return normalized * FullCircleAngle;
    }

    private void PlayResultHit(EQuickTimeEventResult result)
    {
        if (_resultHitImage == null)
        {
            return;
        }

        Color hitColor = ResolveHitColor(result);
        _resultHitTween?.Kill();
        _resultHitImage.color = hitColor;
        SetResultHitAlpha(1f);

        _resultHitTween = DOTween.Sequence()
            .Append(_resultHitImage.DOColor(hitColor, _hitColorDuration).SetEase(_hitEase))
            .Append(_resultHitImage.DOColor(CreateVisibleOriginalHitColor(), _hitFadeDuration).SetEase(_hitEase))
            .SetLink(gameObject);
    }

    private Color ResolveHitColor(EQuickTimeEventResult result)
    {
        switch (result)
        {
            case EQuickTimeEventResult.GreatSuccess:
            {
                return _greatSuccessHitColor;
            }
            case EQuickTimeEventResult.Success:
            {
                return _successHitColor;
            }
            case EQuickTimeEventResult.Fail:
            {
                return _failHitColor;
            }
            default:
            {
                return _originalResultHitColor;
            }
        }
    }

    private void HideAfterFeedback()
    {
        ResetView();
        Hide();
    }

    private void PlayFailShakeIfNeeded(EQuickTimeEventResult result)
    {
        if (result != EQuickTimeEventResult.Fail)
        {
            return;
        }

        if (_feedbackShakeTarget == null)
        {
            return;
        }

        _failShakeTween?.Kill();
        _feedbackShakeTarget.anchoredPosition = _defaultShakeAnchorPosition;
        _failShakeTween = _feedbackShakeTarget
            .DOPunchAnchorPos(
                new Vector2(_failShakeStrength, 0.0f),
                _failShakeDuration,
                _failShakeVibrato,
                _failShakeElasticity)
            .SetLink(gameObject)
            .OnKill(ResetShakeTargetPosition);
    }

    private void SetResultHitAlpha(float alpha)
    {
        if (_resultHitImage == null)
        {
            return;
        }

        Color color = _resultHitImage.color;
        color.a = alpha;
        _resultHitImage.color = color;
    }

    private Color CreateVisibleOriginalHitColor()
    {
        Color color = _originalResultHitColor;
        color.a = 0f;
        return color;
    }

    private void ResetShakeTargetPosition()
    {
        if (_feedbackShakeTarget == null)
        {
            return;
        }

        _feedbackShakeTarget.anchoredPosition = _defaultShakeAnchorPosition;
    }

    private void KillTweens()
    {
        _hideDelayTween?.Kill();
        _hideDelayTween = null;

        _resultHitTween?.Kill();
        _resultHitTween = null;

        _failShakeTween?.Kill();
        _failShakeTween = null;
    }
}
