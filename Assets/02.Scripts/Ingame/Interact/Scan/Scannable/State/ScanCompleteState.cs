public class ScanCompleteState : ScanStateBase
{
    public override EScannableState StateType => EScannableState.OnCompleted;

    public ScanCompleteState(ScannableObject owner) : base(owner)
    {
    }
}
