public class ScanCompleteState : ScanStateBase
{
    public override EScanState StateType => EScanState.OnCompleted;

    public ScanCompleteState(ScannableObject owner) : base(owner)
    {
    }
}
