public class ScanProgressState : ScanStateBase
{
    private float _elapsedTime;

    public override EScannableState StateType => EScannableState.OnProgress;

    public ScanProgressState(ScannableObject owner) : base(owner)
    {
    }

    public override void Enter()
    {
        _elapsedTime = 0.0f;
    }

    public override void OnScanning(float deltaTime)
    {
        _elapsedTime += deltaTime;

        Owner.ApplyScanProgress(deltaTime);
        if (Owner.TryTransitToCompleted())
        {
            return;
        }

        if (_elapsedTime >= Owner.CurrentQTEDelay)
        {
            Owner.ChangeState(EScannableState.OnPlayQTE);
        }
    }

    public override void OnScanLost()
    {
        Owner.ChangeState(EScannableState.OnReturn);
    }
}
