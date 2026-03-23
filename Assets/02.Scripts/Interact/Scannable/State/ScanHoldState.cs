public class ScanHoldState : ScanStateBase
{
    public override EScannableState StateType => EScannableState.OnHold;

    public ScanHoldState(ScannableObject owner) : base(owner)
    {
    }

    public override void OnScanLost()
    {
        Owner.ChangeState(EScannableState.OnReturn);
    }
}
