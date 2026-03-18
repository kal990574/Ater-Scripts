public class LidarProgressState : ILidarTargetState
{
    private readonly LidarTarget _owner;

    public ELidarTargetState StateType => ELidarTargetState.OnProgress;

    public LidarProgressState(LidarTarget owner)
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
        if (_owner.IsProgressComplete)
        {
            _owner.ChangeState(ELidarTargetState.OnCompleted);
        }
    }

    public void OnScanning(float deltaTime)
    {
        _owner.AddProgress(deltaTime);
    }

    public void OnScanLost()
    {
        _owner.ChangeState(ELidarTargetState.OnReturn);
    }
}