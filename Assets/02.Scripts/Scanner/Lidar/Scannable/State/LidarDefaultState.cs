public class LidarDefaultState : LidarTargetStateBase
{
    public override ELidarTargetState StateType => ELidarTargetState.Default;

    public LidarDefaultState(LidarTarget owner) : base(owner)
    {
    }

    public override void OnScanning(float deltaTime)
    {
        Owner.ChangeState(ELidarTargetState.OnProgress);
        Owner.ApplyScanProgress(deltaTime);
    }
}
