using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractTargetShaderModifier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AllInOneShaderController _shaderPropertyController;
    [SerializeField] private LidarTarget _target;

    [Header("Shader Values")]
    [SerializeField] private InteractTargetShaderConfig _config;

    [Header("Hit Blend Tween")]
    [SerializeField] private float _hitBlendPeak = 1.0f;
    [SerializeField] private float _hitBlendUpDuration = 0.08f;
    [SerializeField] private float _hitBlendDownDuration = 0.2f;
    [SerializeField] private Ease _hitBlendUpEase = Ease.OutQuad;
    [SerializeField] private Ease _hitBlendDownEase = Ease.InQuad;

    private Tween _hitBlendTween;
    private float _currentHitBlend;

    private void Awake()
    {
        if (_shaderPropertyController == null)
        {
            _shaderPropertyController = GetComponentInChildren<AllInOneShaderController>();
        }
        _shaderPropertyController.Init();
        SetOutlineColor(_config.AbstractOutlineColor);
        SetGlitchAmountByRatio(1.0f);
        SetBlendCutOffRatio(0.0f);
        SetHitBlend(0.0f);

        if (_target == null)
        {
            _target = GetComponentInParent<LidarTarget>();
        }

        if (_target != null)
        {
            _target.OnProgressChanged += OnTargetProgressChanged;
            _target.OnScanComplete += OnTargetScanComplete;
        }
    }

    private void OnDestroy()
    {
        if (_target != null)
        {
            _target.OnProgressChanged -= OnTargetProgressChanged;
            _target.OnScanComplete -= OnTargetScanComplete;
        }

        KillHitBlendTween();
    }

    private void OnDisable()
    {
        KillHitBlendTween();
    }

    private void OnTargetProgressChanged(float ratio)
    {
        SetBlendCutOffRatio(ratio);
        SetGlitchAmountByRatio(1.0f - ratio);
        SetOutlineColor(Color.Lerp(_config.AbstractOutlineColor, _config.OnHoverOutlineColor, ratio));
    }

    private void OnTargetScanComplete()
    {
        PlayHitBlendEffect();
    }

    private void OnTargetHoverOn()
    {
    }

    private void OnTargetHoverOff()
    {
    }

    private void PlayHitBlendEffect()
    {
        KillHitBlendTween();

        _hitBlendTween = DOTween.Sequence() .Append(DOTween.To(
                () => _currentHitBlend,
                value =>
                {
                    _currentHitBlend = value;
                    SetHitBlend(_currentHitBlend);
                },
                _hitBlendPeak,
                _hitBlendUpDuration))
            .SetEase(_hitBlendUpEase)
            .Append(DOTween.To(
                () => _currentHitBlend,
                value =>
                {
                    _currentHitBlend = value;
                    SetHitBlend(_currentHitBlend);
                },
                0.0f,
                _hitBlendDownDuration))
            .SetEase(_hitBlendDownEase)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable)
            .OnKill(() =>
            {
                _hitBlendTween = null;
            });
    }

    private void KillHitBlendTween()
    {
        if (_hitBlendTween != null && _hitBlendTween.IsActive() == true)
        {
            _hitBlendTween.Kill();
            _hitBlendTween = null;
        }
    }

    private void SetOutlineColor(Color newOutlineColor)
    {
        _shaderPropertyController.SetColor(_config.OutlineColorName, newOutlineColor);
    }

    private void SetBlendCutOffRatio(float ratio)
    {
        _shaderPropertyController.SetFloat(_config.TextureBlendingCutoffName, ratio);
    }

    private void SetGlitchAmountByRatio(float ratio)
    {
        float glitchSpeed = _config.GlitchAmountPower * ratio;
        _shaderPropertyController.SetFloat(_config.GlitchAmountName, glitchSpeed);
    }

    private void SetHitBlend(float power)
    {
        _shaderPropertyController.SetFloat(_config.HitBlendName, power);
    }
}