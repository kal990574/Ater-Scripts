public interface IScanMinigame
{
    bool IsPlaying { get; }
    bool IsFinished { get; }
    MinigameResult Result { get; }

    void Begin();
    void Tick(float deltaTime);
    void Submit();
    void Cancel();
    void ForceFail();
}