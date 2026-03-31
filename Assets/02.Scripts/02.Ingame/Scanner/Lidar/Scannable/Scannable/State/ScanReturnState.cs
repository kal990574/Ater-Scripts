public class ScanReturnState : ScanStateBase
{
    public override EScanState StateType => EScanState.OnReturn;

    public ScanReturnState(ScannableObject owner) : base(owner)
    {
    }

    public override void Tick(float deltaTime)
    {
        Owner.ReduceProgressByReturn(deltaTime);
    }

    public override void OnScanning(float deltaTime)
    {
        Owner.ChangeState(EScanState.OnProgress);
        Owner.ApplyScanProgress(deltaTime);
    }
}
