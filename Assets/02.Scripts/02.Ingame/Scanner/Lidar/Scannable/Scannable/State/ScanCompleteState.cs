public class ScanCompleteState : ScanStateBase
{
    public override EScanState StateType => EScanState.OnCompleted;

    public ScanCompleteState(IScanStateContext owner) : base(owner)
    {
    }
}
