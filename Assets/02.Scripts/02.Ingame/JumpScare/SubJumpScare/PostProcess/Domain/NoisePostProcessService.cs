using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class NoisePostProcessService : IPostProcessEffectService
{
    [SerializeField] private bool _hasSnapshot;

    private FilmGrain _filmGrain;
    private ChromaticAberration _chromaticAberration;
    private LensDistortion _lensDistortion;

    private NoiseSnapshot _snapshot;

    private const float MaxFilmGrainIntensity = 1f;
    private const float MaxFilmGrainResponse = 0f;
    private const float MaxChromaticAberrationIntensity = 1f;
    public void Initialize(PostProcessRuntimeContext context)
    {
        _filmGrain = context.FilmGrain;
        _chromaticAberration = context.ChromaticAberration;
        _lensDistortion = context.LensDistortion;
    }

    public void CaptureSnapshot()
    {
        if (_filmGrain == null || _chromaticAberration == null || _lensDistortion == null)
        {
            return;
        }

        _snapshot = new NoiseSnapshot(
            _filmGrain.active,
            _filmGrain.type.overrideState,
            _filmGrain.type.value,
            _filmGrain.intensity.overrideState,
            _filmGrain.intensity.value,
            _filmGrain.response.overrideState,
            _filmGrain.response.value,
            _chromaticAberration.active,
            _chromaticAberration.intensity.overrideState,
            _chromaticAberration.intensity.value
        );

        _hasSnapshot = true;
    }

    public void Apply(ESubJumpScareIntensity intensity, float effectStrength)
    {
        if (_hasSnapshot == false)
        {
            return;
        }

        float clampedStrength = Mathf.Clamp01(effectStrength);

        _filmGrain.active = true;
        _filmGrain.type.overrideState = true;
        _filmGrain.intensity.overrideState = true;
        _filmGrain.response.overrideState = true;

        _chromaticAberration.active = true;
        _chromaticAberration.intensity.overrideState = true;

        _lensDistortion.active = true;
        _lensDistortion.intensity.overrideState = true;

        _filmGrain.type.value = GetFilmGrainLookup(intensity);

        _filmGrain.intensity.value = Mathf.Lerp(
            _snapshot.FilmGrainIntensity,
            MaxFilmGrainIntensity,
            clampedStrength
        );

        _filmGrain.response.value = Mathf.Lerp(
            _snapshot.FilmGrainResponse,
            MaxFilmGrainResponse,
            clampedStrength
        );

        _chromaticAberration.intensity.value = Mathf.Lerp(
            _snapshot.ChromaticAberrationIntensity,
            MaxChromaticAberrationIntensity,
            clampedStrength
        );
    }

    public void Restore()
    {
        if (_hasSnapshot == false)
        {
            return;
        }

        _filmGrain.active = _snapshot.FilmGrainActive;
        _filmGrain.type.overrideState = _snapshot.FilmGrainTypeOverride;
        _filmGrain.type.value = _snapshot.FilmGrainType;
        _filmGrain.intensity.overrideState = _snapshot.FilmGrainIntensityOverride;
        _filmGrain.intensity.value = _snapshot.FilmGrainIntensity;
        _filmGrain.response.overrideState = _snapshot.FilmGrainResponseOverride;
        _filmGrain.response.value = _snapshot.FilmGrainResponse;

        _chromaticAberration.active = _snapshot.ChromaticAberrationActive;
        _chromaticAberration.intensity.overrideState = _snapshot.ChromaticAberrationIntensityOverride;
        _chromaticAberration.intensity.value = _snapshot.ChromaticAberrationIntensity;
        _hasSnapshot = false;
    }

    private FilmGrainLookup GetFilmGrainLookup(ESubJumpScareIntensity intensity)
    {
        switch (intensity)
        {
            case ESubJumpScareIntensity.Weak:
            {
                return FilmGrainLookup.Thin1;
            }
            case ESubJumpScareIntensity.Medium:
            {
                return FilmGrainLookup.Medium3;
            }
            case ESubJumpScareIntensity.Strong:
            {
                return FilmGrainLookup.Large02;
            }
            default:
            {
                return FilmGrainLookup.Thin1;
            }
        }
    }

    [System.Serializable]
    private struct NoiseSnapshot
    {
        public bool FilmGrainActive;
        public bool FilmGrainTypeOverride;
        public FilmGrainLookup FilmGrainType;
        public bool FilmGrainIntensityOverride;
        public float FilmGrainIntensity;
        public bool FilmGrainResponseOverride;
        public float FilmGrainResponse;

        public bool ChromaticAberrationActive;
        public bool ChromaticAberrationIntensityOverride;
        public float ChromaticAberrationIntensity;

        public NoiseSnapshot(
            bool filmGrainActive,
            bool filmGrainTypeOverride,
            FilmGrainLookup filmGrainType,
            bool filmGrainIntensityOverride,
            float filmGrainIntensity,
            bool filmGrainResponseOverride,
            float filmGrainResponse,
            bool chromaticAberrationActive,
            bool chromaticAberrationIntensityOverride,
            float chromaticAberrationIntensity)
        {
            FilmGrainActive = filmGrainActive;
            FilmGrainTypeOverride = filmGrainTypeOverride;
            FilmGrainType = filmGrainType;
            FilmGrainIntensityOverride = filmGrainIntensityOverride;
            FilmGrainIntensity = filmGrainIntensity;
            FilmGrainResponseOverride = filmGrainResponseOverride;
            FilmGrainResponse = filmGrainResponse;
            ChromaticAberrationActive = chromaticAberrationActive;
            ChromaticAberrationIntensityOverride = chromaticAberrationIntensityOverride;
            ChromaticAberrationIntensity = chromaticAberrationIntensity;
        }
    }
}