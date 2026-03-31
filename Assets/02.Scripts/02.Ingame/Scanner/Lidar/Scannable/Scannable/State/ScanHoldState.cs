public class ScanHoldState : ScanStateBase
{
    public override EScanState StateType => EScanState.OnHold;

    public ScanHoldState(ScannableObject owner) : base(owner)
    {
    }

    public override void OnScanLost()
    {
        Owner.ChangeState(EScanState.OnReturn);
    }
}
