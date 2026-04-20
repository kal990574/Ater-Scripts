public interface IPuzzleInputHandler
{
    EPuzzleType PuzzleType { get; }
    void ConfirmActivePuzzle();
    void CancelActivePuzzle();
}

public interface IPuzzleIntance
{
    void TryEvaluate();
    void Cancel();
}
