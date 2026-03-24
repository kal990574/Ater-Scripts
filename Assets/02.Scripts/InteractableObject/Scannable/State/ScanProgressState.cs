public class ScanProgressState : ScanStateBase
{
    public override EScannableState StateType => EScannableState.OnProgress;

    public ScanProgressState(InteractScanObject owner) : base(owner)
    {
    }

    public override void OnScanning(float deltaTime)
    {
        Owner.ApplyScanProgress(deltaTime);
        Owner.TryTransitToCompleted();
    }

    public override void OnScanLost()
    {
        Owner.ChangeState(EScannableState.OnReturn);
    }
}
