public class ScanCompleteState : ScanStateBase
{
    public override EScannableState StateType => EScannableState.OnCompleted;

    public ScanCompleteState(InteractScanObject owner) : base(owner)
    {
    }
}
