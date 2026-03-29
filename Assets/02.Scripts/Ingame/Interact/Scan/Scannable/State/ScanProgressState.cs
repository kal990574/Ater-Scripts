public class ScanProgressState : ScanStateBase
{
    public override EScanState StateType => EScanState.OnProgress;

    public ScanProgressState(ScannableObject owner) : base(owner)
    {
    }

    public override void OnScanning(float deltaTime)
    {
        Owner.ApplyScanProgress(deltaTime);
        Owner.TryTransitToCompleted();
    }

    public override void OnScanLost()
    {
        Owner.ChangeState(EScanState.OnReturn);
    }
}
