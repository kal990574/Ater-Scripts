public class LidarReturnState : LidarTargetStateBase
{
    public override ELidarTargetState StateType => ELidarTargetState.OnReturn;

    public LidarReturnState(LidarTarget owner) : base(owner)
    {
    }

    public override void Tick(float deltaTime)
    {
        Owner.ReduceProgressByReturn(deltaTime);
    }

    public override void OnScanning(float deltaTime)
    {
        Owner.ChangeState(ELidarTargetState.OnProgress);
        Owner.ApplyScanProgress(deltaTime);
    }
}
