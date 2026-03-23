public class ScanDefaultState : ScanStateBase
{
    public override EScannableState StateType => EScannableState.Default;

    public ScanDefaultState(ScannableObject owner) : base(owner)
    {
    }

    public override void OnScanning(float deltaTime)
    {
        Owner.ChangeState(EScannableState.OnProgress);
        Owner.ApplyScanProgress(deltaTime);
    }
}
