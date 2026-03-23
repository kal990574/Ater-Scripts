public class LidarCompleteState : LidarTargetStateBase
{
    public override ELidarTargetState StateType => ELidarTargetState.OnCompleted;

    public LidarCompleteState(LidarTarget owner) : base(owner)
    {
    }
}
