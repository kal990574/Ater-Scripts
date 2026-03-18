public class LidarReturnState : ILidarTargetState
{
    private readonly LidarTarget _owner;

    public ELidarTargetState StateType => ELidarTargetState.OnReturn;

    public LidarReturnState(LidarTarget owner)
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
        _owner.ReduceProgressByReturn(deltaTime);

        if (_owner.CurrentProgress <= 0)
        {
            _owner.ChangeState(ELidarTargetState.Default);
        }
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