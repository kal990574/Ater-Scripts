public class LidarDefaultState : ILidarTargetState
{
    private readonly LidarTarget _owner;

    public ELidarTargetState StateType => ELidarTargetState.Default;

    public LidarDefaultState(LidarTarget owner)
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
        _owner.ChangeState(ELidarTargetState.OnProgress);
        _owner.AddProgress(deltaTime);
    }

    public void OnScanLost()
    {
    }
}