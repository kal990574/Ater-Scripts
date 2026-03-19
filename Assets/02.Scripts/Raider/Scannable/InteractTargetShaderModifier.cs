using DG.Tweening;
using UnityEngine;

public class InteractTargetShaderModifier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AllInOneShaderController _shaderPropertyController;
    [SerializeField] private LidarTarget _target;

    [Header("Shader Values")]
    [SerializeField] private InteractTargetShaderConfig _config;

    private Tween _hitBlendTween;
    private float _currentHitBlend;
    private bool _isInitialized;

    private void Start()
    {
        CacheReferences();
        InitializeShaderController();
        ApplyInitialShaderState();
        SubscribeTargetEvents();
        _isInitialized = true;
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

        if (_target == null)
        {
            _target = GetComponentInParent<LidarTarget>();
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
        SetOutlineColor(_config.AbstractOutlineColor);
        ApplyProgressState(0.0f);
        SetHitBlend(0.0f);
    }

    private void SubscribeTargetEvents()
    {
        if (_target == null)
        {
            return;
        }
        
        _target.OnProgressChanged += OnTargetProgressChanged;
        _target.OnScanComplete += OnTargetScanComplete;
    }

    private void UnsubscribeTargetEvents()
    {
        if (_target == null)
        {
            return;
        }

        _target.OnProgressChanged -= OnTargetProgressChanged;
        _target.OnScanComplete -= OnTargetScanComplete;
    }

    private void OnTargetProgressChanged(float ratio)
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
        UpdateOptionalEffects(1.0f- ratio);
        UpdateOutlineColor(ratio);
    }

    private void UpdateOptionalEffects(float inverseRatio)
    {
        if (_config.ActiveGlitch)
        {
            SetGlitchAmount(inverseRatio);
        }

        if (_config.ActiveDistortion)
        {
            SetDistortionAmount(inverseRatio);
        }
    }

    private void UpdateOutlineColor(float ratio)
    {
        Color outlineColor = Color.Lerp(_config.AbstractOutlineColor, _config.OnHoverOutlineColor, ratio);
        SetOutlineColor(outlineColor);
    }

    private void PlayHitBlendEffect()
    {
        KillHitBlendTween();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(CreateHitBlendTween(_config.HitBlendPeak, _config.HitBlendDuration)
            .SetEase(_config.HitBlendUpEase));

        sequence.Append(CreateHitBlendTween(0.0f, _config.HitBlendDownDuration)
            .SetEase(_config.HitBlendDownEase));

        sequence.SetLink(gameObject, LinkBehaviour.KillOnDisable);
        sequence.OnKill(() =>
        {
            _hitBlendTween = null;
        });

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

        if (_hitBlendTween.IsActive() == true)
        {
            _hitBlendTween.Kill();
        }

        _hitBlendTween = null;
    }

    private void SetOutlineColor(Color value)
    {
        _shaderPropertyController.SetColor(_config.OutlineColorName, value);
    }

    private void SetBlendCutOffRatio(float value)
    {
        _shaderPropertyController.SetFloat(_config.TextureBlendingCutoffName, value);
    }

    private void SetGlitchAmount(float ratio)
    {
        float glitchAmount = _config.GlitchAmountPower * ratio;
        _shaderPropertyController.SetFloat(_config.GlitchAmountName, glitchAmount);
    }

    private void SetDistortionAmount(float ratio)
    {
        float distortionAmount = _config.DistortionAmountPower * ratio;
        _shaderPropertyController.SetFloat(_config.DistortionAmountName, distortionAmount);
    }

    private void SetHitBlend(float value)
    {
        _shaderPropertyController.SetFloat(_config.HitBlendName, value);
    }
}