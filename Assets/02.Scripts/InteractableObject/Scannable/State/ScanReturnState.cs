public class ScanReturnState : ScanStateBase
{
    public override EScannableState StateType => EScannableState.OnReturn;

    public ScanReturnState(InteractScanObject owner) : base(owner)
    {
    }

    public override void Tick(float deltaTime)
    {
        Owner.ReduceProgressByReturn(deltaTime);
    }

    public override void OnScanning(float deltaTime)
    {
        Owner.ChangeState(EScannableState.OnProgress);
        Owner.ApplyScanProgress(deltaTime);
    }
}
