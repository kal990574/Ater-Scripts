public class LidarCompleteState : ILidarTargetState
{
    private readonly LidarTarget _owner;

    public ELidarTargetState StateType => ELidarTargetState.OnCompleted;

    public LidarCompleteState(LidarTarget owner)
    {
        _owner = owner;
    }

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Tick(float deltaTime)
    {
    }

    public void OnScanning(float deltaTime)
    {
    }

    public void OnScanLost()
    {
    }
}