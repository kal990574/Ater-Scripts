using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LidarScanProgressUI : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fillImage;
    [SerializeField] private LidarScanFeature _scanFeature;

    [Header("Progress Tween")]
    [SerializeField] private float _qteProgressTweenDuration = 0.2f;
    [SerializeField] private Ease _qteProgressEase = Ease.OutCubic;

    [Header("QTE Hit Effect")]
    [SerializeField] private Color _defaultFillColor = Color.white;
    [SerializeField] private Color _qteFailHitColor = Color.red;
    [SerializeField] private Color _qteGreatSuccessHitColor = Color.green;
    [SerializeField] private float _hitColorDuration = 0.12f;
    [SerializeField] private float _hitColorReturnDuration = 0.2f;
    [SerializeField] private Ease _hitColorEase = Ease.OutQuad;

    private ScannableObject _currentTarget;
    private ScannableQTEInvoker _currentQteInvoker;
    private Tween _progressTween;
    private Tween _fillColorTween;
    private bool _shouldTweenNextProgressChange;

    private void Awake()
    {
        if (_scanFeature == null)
        {
            _scanFeature = FindFirstObjectByType<LidarScanFeature>();
        }

        if (_slider == null && TryResolveSlider() == false)
        {
            DisableSelf();
            return;
        }

        if (_fillImage == null)
        {
            _fillImage = _slider.fillRect != null ? _slider.fillRect.GetComponent<Image>() : null;
        }

        if (_scanFeature == null || _slider == null || _canvasGroup == null)
        {
            DisableSelf();
            return;
        }

        if (_fillImage != null)
        {
            _defaultFillColor = _fillImage.color;
        }

        _scanFeature.OnTargetFind += SetTarget;
        _scanFeature.OnTargetLost += ResetTarget;
        SetVisible(false);
    }

    private void OnDestroy()
    {
        if (_scanFeature != null)
        {
            _scanFeature.OnTargetFind -= SetTarget;
            _scanFeature.OnTargetLost -= ResetTarget;
        }

        KillTweens();
        UnbindCurrentTarget();
    }

    public void SetTarget(ScannableObject target)
    {
        if (target == null)
        {
            ResetTarget();
            return;
        }

        if (_currentTarget == target)
        {
            Refresh(target.ProgressRatio, false);
            SetVisible(true);
            return;
        }

        UnbindCurrentTarget();
        _currentTarget = target;
        _currentTarget.OnScanProgressChanged += HandleProgressChanged;
        BindQteInvoker(_currentTarget);
        Refresh(_currentTarget.ProgressRatio, false);
        SetVisible(true);
    }

    public void ResetTarget()
    {
        UnbindCurrentTarget();
        Refresh(0f, false);
        SetVisible(false);
    }

    public void Refresh(float ratio = 0f, bool useTween = false)
    {
        if (_slider == null)
        {
            return;
        }

        float clampedRatio = Mathf.Clamp01(ratio);
        if (!useTween)
        {
            _progressTween?.Kill();
            _slider.value = clampedRatio;
            return;
        }

        _progressTween?.Kill();
        _progressTween = _slider
            .DOValue(clampedRatio, _qteProgressTweenDuration)
            .SetEase(_qteProgressEase)
            .SetLink(gameObject);
    }

    private void UnbindCurrentTarget()
    {
        UnbindQteInvoker();
        _shouldTweenNextProgressChange = false;

        if (_currentTarget == null)
        {
            return;
        }

        _currentTarget.OnScanProgressChanged -= HandleProgressChanged;
        _currentTarget = null;
    }

    private void HandleProgressChanged(float ratio)
    {
        bool useTween = _shouldTweenNextProgressChange;
        _shouldTweenNextProgressChange = false;
        Refresh(ratio, useTween);
    }

    private void BindQteInvoker(ScannableObject target)
    {
        if (target == null)
        {
            return;
        }

        _currentQteInvoker = target.GetComponent<ScannableQTEInvoker>();
        if (_currentQteInvoker == null)
        {
            _currentQteInvoker = target.GetComponentInChildren<ScannableQTEInvoker>();
        }

        if (_currentQteInvoker == null)
        {
            return;
        }

        _currentQteInvoker.OnQteFailed += HandleQteFailed;
        _currentQteInvoker.OnQteGreatSucceeded += HandleQteGreatSucceeded;
    }

    private void UnbindQteInvoker()
    {
        if (_currentQteInvoker == null)
        {
            return;
        }

        _currentQteInvoker.OnQteFailed -= HandleQteFailed;
        _currentQteInvoker.OnQteGreatSucceeded -= HandleQteGreatSucceeded;
        _currentQteInvoker = null;
    }

    private void HandleQteFailed()
    {
        _shouldTweenNextProgressChange = true;
        PlayFillHit(_qteFailHitColor);
    }

    private void HandleQteGreatSucceeded()
    {
        PlayFillHit(_qteGreatSuccessHitColor);
    }

    private void PlayFillHit(Color hitColor)
    {
        if (_fillImage == null)
        {
            return;
        }

        _fillColorTween?.Kill();
        _fillImage.color = _defaultFillColor;

        _fillColorTween = DOTween.Sequence()
            .Append(_fillImage.DOColor(hitColor, _hitColorDuration).SetEase(_hitColorEase))
            .Append(_fillImage.DOColor(_defaultFillColor, _hitColorReturnDuration).SetEase(_hitColorEase))
            .SetLink(gameObject);
    }

    private bool TryResolveSlider()
    {
        _slider = GetComponent<Slider>();
        return _slider != null;
    }

    private void KillTweens()
    {
        _progressTween?.Kill();
        _fillColorTween?.Kill();
    }

    private void DisableSelf()
    {
        enabled = false;
        SetVisible(false);
    }

    private void SetVisible(bool isVisible)
    {
        if (_canvasGroup == null)
        {
            return;
        }

        _canvasGroup.alpha = isVisible ? 1.0f : 0.0f;
        _canvasGroup.interactable = isVisible;
        _canvasGroup.blocksRaycasts = isVisible;
    }
}
