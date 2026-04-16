public interface IPuzzleInputHandler
{
    void ConfirmActivePuzzle();
    void CancelActivePuzzle();
}

public interface IPuzzleIntance
{
    void TryEvaluate();
    void Cancel();
}
