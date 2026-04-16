public interface IScanStateContext
{
    EScanState State { get; }
    bool IsProgressComplete { get; }
    float CurrentProgress { get; }

    void ChangeState(EScanState nextState, bool force = false);
    void ApplyScanProgress(float deltaTime);
    void ReduceProgressByReturn(float deltaTime);
    bool TryTransitToCompleted();
}
