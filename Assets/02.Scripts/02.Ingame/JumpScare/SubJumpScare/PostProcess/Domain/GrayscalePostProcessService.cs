using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class GrayscalePostProcessService : IPostProcessEffectService
{
    [SerializeField] private bool _hasSnapshot;

    private ColorAdjustments _colorAdjustments;
    private GrayscaleSnapshot _snapshot;

    private const float MaxGrayscaleSaturation = -100.0f;

    public void Initialize(PostProcessRuntimeContext context)
    {
        _colorAdjustments = context.ColorAdjustments;
    }

    public void CaptureSnapshot()
    {
        if (_colorAdjustments == null)
        {
            return;
        }

        _snapshot = new GrayscaleSnapshot(
            _colorAdjustments.active,
            _colorAdjustments.saturation.overrideState,
            _colorAdjustments.saturation.value
        );

        _hasSnapshot = true;
    }

    public void Apply(float effectStrength)
    {
        if (_hasSnapshot == false)
        {
            return;
        }

        float clampedStrength = Mathf.Clamp01(effectStrength);

        _colorAdjustments.active = true;
        _colorAdjustments.saturation.overrideState = true;
        _colorAdjustments.saturation.value = Mathf.Lerp(
            _snapshot.Saturation,
            MaxGrayscaleSaturation,
            clampedStrength
        );
    }

    public void Restore()
    {
        if (_hasSnapshot == false)
        {
            return;
        }

        _colorAdjustments.active = _snapshot.ColorAdjustmentsActive;
        _colorAdjustments.saturation.overrideState = _snapshot.SaturationOverride;
        _colorAdjustments.saturation.value = _snapshot.Saturation;

        _hasSnapshot = false;
    }

    [System.Serializable]
    private struct GrayscaleSnapshot
    {
        public bool ColorAdjustmentsActive;
        public bool SaturationOverride;
        public float Saturation;

        public GrayscaleSnapshot(
            bool colorAdjustmentsActive,
            bool saturationOverride,
            float saturation)
        {
            ColorAdjustmentsActive = colorAdjustmentsActive;
            SaturationOverride = saturationOverride;
            Saturation = saturation;
        }
    }
}