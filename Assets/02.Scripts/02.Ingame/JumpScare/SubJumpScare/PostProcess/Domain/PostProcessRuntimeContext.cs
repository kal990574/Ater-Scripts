using UnityEngine.Rendering.Universal;

public readonly struct PostProcessRuntimeContext
{
    public Vignette Vignette { get; }
    public ColorAdjustments ColorAdjustments { get; }
    public FilmGrain FilmGrain { get; }
    public ChromaticAberration ChromaticAberration { get; }
    public LensDistortion LensDistortion { get; }

    public PostProcessRuntimeContext(
        Vignette vignette,
        ColorAdjustments colorAdjustments,
        FilmGrain filmGrain,
        ChromaticAberration chromaticAberration,
        LensDistortion lensDistortion)
    {
        Vignette = vignette;
        ColorAdjustments = colorAdjustments;
        FilmGrain = filmGrain;
        ChromaticAberration = chromaticAberration;
        LensDistortion = lensDistortion;
    }
}