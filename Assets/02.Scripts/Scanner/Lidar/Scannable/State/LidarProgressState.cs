public class LidarProgressState : LidarTargetStateBase
{
    private float _elapsedTime;

    public override ELidarTargetState StateType => ELidarTargetState.OnProgress;

    public LidarProgressState(LidarTarget owner) : base(owner)
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
            Owner.ChangeState(ELidarTargetState.OnPlayQTE);
        }
    }

    public override void OnScanLost()
    {
        Owner.ChangeState(ELidarTargetState.OnReturn);
    }
}
