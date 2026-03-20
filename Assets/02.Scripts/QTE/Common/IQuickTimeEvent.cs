public interface IQuickTimeEvent
{
    bool IsPlaying { get; }
    bool IsFinished { get; }
    EQuickTimeEventResult Result { get; }

    void Begin();
    void Tick(float deltaTime);
    void Submit();
    void Cancel();
}