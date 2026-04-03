using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class DarknessPostProcessService : IPostProcessEffectService
{
    [SerializeField] private bool _hasSnapshot;

    private Vignette _vignette;
    private ColorAdjustments _colorAdjustments;
    private DarknessSnapshot _snapshot;

    private const float MaxVignetteIntensity = 0.65f;
    private const float MaxVignetteSmoothness = 0.45f;
    private const float MaxDarkPostExposure = -6f;

    public void Initialize(PostProcessRuntimeContext context)
    {
        _vignette = context.Vignette;
        _colorAdjustments = context.ColorAdjustments;
    }

    public void CaptureSnapshot()
    {
        if (_vignette == null || _colorAdjustments == null)
        {
            return;
        }

        _snapshot = new DarknessSnapshot(
            _vignette.active,
            _vignette.intensity.overrideState,
            _vignette.intensity.value,
            _vignette.smoothness.overrideState,
            _vignette.smoothness.value,
            _colorAdjustments.active,
            _colorAdjustments.postExposure.overrideState,
            _colorAdjustments.postExposure.value
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

        _vignette.active = true;
        _vignette.intensity.overrideState = true;
        _vignette.smoothness.overrideState = true;

        _colorAdjustments.active = true;
        _colorAdjustments.postExposure.overrideState = true;

        _vignette.intensity.value = Mathf.Lerp(
            _snapshot.VignetteIntensity,
            MaxVignetteIntensity,
            clampedStrength
        );

        _vignette.smoothness.value = Mathf.Lerp(
            _snapshot.VignetteSmoothness,
            MaxVignetteSmoothness,
            clampedStrength
        );

        _colorAdjustments.postExposure.value = Mathf.Lerp(
            _snapshot.PostExposure,
            MaxDarkPostExposure,
            clampedStrength
        );
    }

    public void Restore()
    {
        if (_hasSnapshot == false)
        {
            return;
        }

        _vignette.active = _snapshot.VignetteActive;
        _vignette.intensity.overrideState = _snapshot.VignetteIntensityOverride;
        _vignette.intensity.value = _snapshot.VignetteIntensity;
        _vignette.smoothness.overrideState = _snapshot.VignetteSmoothnessOverride;
        _vignette.smoothness.value = _snapshot.VignetteSmoothness;

        _colorAdjustments.active = _snapshot.ColorAdjustmentsActive;
        _colorAdjustments.postExposure.overrideState = _snapshot.PostExposureOverride;
        _colorAdjustments.postExposure.value = _snapshot.PostExposure;

        _hasSnapshot = false;
    }

    [System.Serializable]
    private struct DarknessSnapshot
    {
        public bool VignetteActive;
        public bool VignetteIntensityOverride;
        public float VignetteIntensity;
        public bool VignetteSmoothnessOverride;
        public float VignetteSmoothness;

        public bool ColorAdjustmentsActive;
        public bool PostExposureOverride;
        public float PostExposure;

        public DarknessSnapshot(
            bool vignetteActive,
            bool vignetteIntensityOverride,
            float vignetteIntensity,
            bool vignetteSmoothnessOverride,
            float vignetteSmoothness,
            bool colorAdjustmentsActive,
            bool postExposureOverride,
            float postExposure)
        {
            VignetteActive = vignetteActive;
            VignetteIntensityOverride = vignetteIntensityOverride;
            VignetteIntensity = vignetteIntensity;
            VignetteSmoothnessOverride = vignetteSmoothnessOverride;
            VignetteSmoothness = vignetteSmoothness;
            ColorAdjustmentsActive = colorAdjustmentsActive;
            PostExposureOverride = postExposureOverride;
            PostExposure = postExposure;
        }
    }
}