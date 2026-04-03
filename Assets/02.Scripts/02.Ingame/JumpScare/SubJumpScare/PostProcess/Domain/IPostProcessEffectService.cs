public interface IPostProcessEffectService
{
    void Initialize(PostProcessRuntimeContext context);
    void CaptureSnapshot();
    void Restore();
}