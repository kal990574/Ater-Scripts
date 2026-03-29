public class ScanDefaultState : ScanStateBase
{
    public override EScanState StateType => EScanState.Default;

    public ScanDefaultState(ScannableObject owner) : base(owner)
    {
    }

    public override void OnScanning(float deltaTime)
    {
        Owner.ChangeState(EScanState.OnProgress);
        Owner.ApplyScanProgress(deltaTime);
    }
}
