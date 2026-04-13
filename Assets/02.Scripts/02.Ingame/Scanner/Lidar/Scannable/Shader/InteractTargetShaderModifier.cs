using _02.Scripts.Sonar;
using DG.Tweening;
using UnityEngine;

public class InteractTargetShaderModifier : MonoBehaviour
{
    public const string TEXTURE_BLENDING_CUTOFF_NAME = "_BlendingMaskCutoffWhite";
    public const string OUTLINE_COLOR_NAME = "_OutlineColor";
    public const string OUTLINE_THICKNESS_NAME = "_OutlineThickness";
    public const string GLITCH_AMOUNT_NAME = "_GlitchAmount";
    public const string DISTORTION_AMOUNT_NAME = "_VertexDistortionAmount";
    public const string HIT_BLEND_NAME = "_HitBlend";
    [Header("Required References")]
    [SerializeField] private AllInOneShaderController _shaderPropertyController;
    [SerializeField] private ScanShaderConfigSO _scanConfig;
    [SerializeField] private OutlineShaderConfigSO _oultineConfig;
    
    private IScannable scannable;
    private IDetectable detectable;
    private SonarHighlightShaderModifier _sonarModifier;

    private Tween _hitBlendTween;
    private float _currentHitBlend;
    private bool _isDetected;
    private float _currentScanRatio;
    
    private void Start()
    {
        CacheReferences();
        InitShaderController();
        ApplyInitialShaderState();
        SubscribeTargetEvents();
        SynchronizeCurrentState();
    }

    private void OnDestroy()
    {
        UnsubscribeTargetEvents();
        KillHitBlendTween();
    }

    private void CacheReferences()
    {
        if (_shaderPropertyController == null)
        {
            _shaderPropertyController = GetComponentInChildren<AllInOneShaderController>();
        }

        if (scannable == null)
        {
            scannable = GetComponentInParent<ScannableObject>();
        }

        if (detectable == null)
        {
            detectable = GetComponentInParent<IDetectable>();
        }

        if (_sonarModifier == null)
        {
            _sonarModifier = GetComponent<SonarHighlightShaderModifier>();
        }
    }

    private void InitShaderController()
    {
        if (_shaderPropertyController == null)
        {
            enabled = false;
            return;
        }

        _shaderPropertyController.Init();
    }

    private void ApplyInitialShaderState()
    {
        ApplyOutlineConfig();

        Color hoverOutlineColor = _oultineConfig != null ? _oultineConfig.OnHoverOutlineColor : Color.white;
        Color abstractOutlineColor = _oultineConfig != null ? _oultineConfig.AbstractOutlineColor : Color.white;

        if (scannable == null)
        {
            SetOutlineColor(hoverOutlineColor);
            ApplyProgressState(1f);
        }
        else if (scannable.IsProgressComplete)
        {
            SetOutlineColor(hoverOutlineColor);
            ApplyProgressState(1f);
        }
        else
        {
            SetOutlineColor(abstractOutlineColor);
            ApplyProgressState(0.0f);
        }
        
        RefreshOutlineState();
        SetHitBlend(0.0f);
    }

    private void SubscribeTargetEvents()
    {
        if (scannable != null)
        {
            scannable.OnScanProgressChanged += OnScanTargetProgressChanged;
            scannable.OnScanComplete += OnTargetScanComplete;
        }

        if (detectable != null)
        {
            detectable.OnDetected += ShowOutline;
        }
    }

    private void UnsubscribeTargetEvents()
    {
        if (scannable != null)
        {
            scannable.OnScanProgressChanged -= OnScanTargetProgressChanged;
            scannable.OnScanComplete -= OnTargetScanComplete;
        }

        if (detectable != null)
        {
            detectable.OnDetected -= ShowOutline;
        }
    }

    private void OnScanTargetProgressChanged(float ratio)
    {
        ApplyProgressState(ratio);
    }

    private void OnTargetScanComplete()
    {
        PlayHitBlendEffect();
    }

    public void SynchronizeCurrentState()
    {
        if (scannable != null)
        {
            ApplyProgressState(scannable.ProgressRatio);
        }

        RefreshOutlineState();
    }

    private bool IsSonarHighlighting()
    {
        return _sonarModifier != null && _sonarModifier.IsHighlighting;
    }

    private void ApplyProgressState(float ratio)
    {
        _currentScanRatio = ratio;
        SetBlendCutOffRatio(ratio);
        UpdateOptionalEffects(1.0f - ratio);

        if (IsSonarHighlighting()) return;

        UpdateOutlineColor(ratio);
        RefreshOutlineState();
    }

    private void UpdateOptionalEffects(float inverseRatio)
    {
        if (_scanConfig.ActiveGlitch)
        {
            SetGlitchAmount(inverseRatio);
        }

        if (_scanConfig.ActiveDistortion)
        {
            SetDistortionAmount(inverseRatio);
        }
    }

    private void UpdateOutlineColor(float ratio)
    {
        Color from = _oultineConfig != null ? _oultineConfig.AbstractOutlineColor : Color.white;
        Color to = _oultineConfig != null ? _oultineConfig.OnHoverOutlineColor : Color.white;
        Color outlineColor = Color.Lerp(from, to, ratio);
        SetOutlineColor(outlineColor);
    }

    private void PlayHitBlendEffect()
    {
        if (IsSonarHighlighting()) return;

        KillHitBlendTween();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(CreateHitBlendTween(_scanConfig.HitBlendPeak, _scanConfig.HitBlendDuration).SetEase(_scanConfig.HitBlendUpEase));
        sequence.Append(CreateHitBlendTween(0.0f, _scanConfig.HitBlendDownDuration).SetEase(_scanConfig.HitBlendDownEase));
        sequence.SetLink(gameObject, LinkBehaviour.KillOnDisable);
        sequence.OnKill(() => _hitBlendTween = null);

        _hitBlendTween = sequence;
    }

    private Tween CreateHitBlendTween(float targetValue, float duration)
    {
        return DOTween.To(
            () => _currentHitBlend,
            value =>
            {
                _currentHitBlend = value;
                SetHitBlend(value);
            },
            targetValue,
            duration);
    }

    private void KillHitBlendTween()
    {
        if (_hitBlendTween == null)
        {
            return;
        }

        if (_hitBlendTween.IsActive())
        {
            _hitBlendTween.Kill();
        }

        _hitBlendTween = null;
    }

    private void SetOutlineColor(Color value)
    {
        _shaderPropertyController.SetColor(OUTLINE_COLOR_NAME, value);
    }

    private void ApplyOutlineConfig()
    {
        if (_oultineConfig == null)
        {
            return;
        }

        _shaderPropertyController.SetFloat(OUTLINE_THICKNESS_NAME, _oultineConfig.GetOutlineThickness());
    }

    private void SetBlendCutOffRatio(float value)
    {
        _shaderPropertyController.SetFloat(TEXTURE_BLENDING_CUTOFF_NAME, value);
    }

    private void SetGlitchAmount(float ratio)
    {
        float glitchAmount = _scanConfig.GlitchAmountPower * ratio;
        _shaderPropertyController.SetFloat(GLITCH_AMOUNT_NAME, glitchAmount);
    }

    private void SetDistortionAmount(float ratio)
    {
        float distortionAmount = _scanConfig.DistortionAmountPower * ratio;
        _shaderPropertyController.SetFloat(DISTORTION_AMOUNT_NAME, distortionAmount);
    }

    private void SetHitBlend(float value)
    {
        _shaderPropertyController.SetFloat(HIT_BLEND_NAME, value);
    }

    private void ShowOutline(bool show)
    {
        _isDetected = show;

        if (IsSonarHighlighting()) return;

        RefreshOutlineState();
    }

    private void RefreshOutlineState()
    {
        bool shouldShowOutline = _isDetected || IsScanInProgress();
        AllInOneShaderController.OutlineType outlineType = shouldShowOutline
            ? ConvertOutlineType(_oultineConfig != null ? _oultineConfig.Type : OutlineShaderConfigSO.OutlineType.Simple)
            : AllInOneShaderController.OutlineType.None;

        _shaderPropertyController.SetOutlineType(outlineType);
    }

    private bool IsScanInProgress()
    {
        return _currentScanRatio > 0.0f && _currentScanRatio < 1.0f;
    }

    private static AllInOneShaderController.OutlineType ConvertOutlineType(OutlineShaderConfigSO.OutlineType outlineType)
    {
        return outlineType switch
        {
            OutlineShaderConfigSO.OutlineType.Simple => AllInOneShaderController.OutlineType.Simple,
            OutlineShaderConfigSO.OutlineType.Constant => AllInOneShaderController.OutlineType.Constant,
            OutlineShaderConfigSO.OutlineType.FadeWithDistance => AllInOneShaderController.OutlineType.FadeWithDistance,
            _ => AllInOneShaderController.OutlineType.None
        };
    }
}
