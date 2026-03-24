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
    
    private IInteractScan _interactScan;
    private IInteractTarget _interactTarget;
    
    [Header("Debug")]
    [SerializeField] private float _currentHitBlend;

    private Tween _hitBlendTween;

    private void Start()
    {
        CacheReferences();
        InitializeShaderController();

        if (enabled == false || _scanConfig == null)
        {
            if (_scanConfig == null)
            {
                Debug.LogError($"[{nameof(InteractTargetShaderModifier)}] {nameof(ScanShaderConfigSO)} is missing.", this);
            }

            enabled = false;
            return;
        }

        ApplyInitialShaderState();
        SubscribeTargetEvents();
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

        if (_interactScan == null)
        {
            _interactScan = GetComponentInParent<InteractScanObject>();
        }

        if (_interactTarget == null)
        {
            _interactTarget = GetComponentInParent<IInteractTarget>();
        }
    }

    private void InitializeShaderController()
    {
        if (_shaderPropertyController == null)
        {
            Debug.LogError($"[{nameof(InteractTargetShaderModifier)}] {nameof(AllInOneShaderController)} is missing.", this);
            enabled = false;
            return;
        }

        _shaderPropertyController.Init();
    }

    private void ApplyInitialShaderState()
    {
        SetOutlineColor(_oultineConfig.AbstractOutlineColor);
        ShowOutline(false);
        ApplyProgressState(0.0f);
        SetHitBlend(0.0f);
    }

    private void SubscribeTargetEvents()
    {
        if (_interactScan != null)
        {
            _interactScan.OnScanProgressChanged += OnScanTargetProgressChanged;
            _interactScan.OnScanComplete += OnTargetScanComplete;
        }

        if (_interactTarget != null)
        {
            _interactTarget.OnTargetDetected += ShowOutline;
        }
    }

    private void UnsubscribeTargetEvents()
    {
        if (_interactScan != null)
        {
            _interactScan.OnScanProgressChanged -= OnScanTargetProgressChanged;
            _interactScan.OnScanComplete -= OnTargetScanComplete;
        }

        if (_interactTarget != null)
        {
            _interactTarget.OnTargetDetected -= ShowOutline;
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

    private void ApplyProgressState(float ratio)
    {
        SetBlendCutOffRatio(ratio);
        UpdateOptionalEffects(1.0f - ratio);
        UpdateOutlineColor(ratio);
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
        Color outlineColor = Color.Lerp(_oultineConfig.AbstractOutlineColor, _oultineConfig.OnHoverOutlineColor, ratio);
        SetOutlineColor(outlineColor);
    }

    private void PlayHitBlendEffect()
    {
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
                SetHitBlend(_currentHitBlend);
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
        Debug.Log($"아웃라인 {show}");
        _shaderPropertyController.SetOutlineEnabled(show);
    }
}
