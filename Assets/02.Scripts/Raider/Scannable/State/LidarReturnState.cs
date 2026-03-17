public class LidarReturnState : ILidarScannableState
{
    private readonly LidarScannableObject _owner;

    public ELidarObjectState StateType => ELidarObjectState.OnReturn;

    public LidarReturnState(LidarScannableObject owner)
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
    }

    public void OnScanning(float deltaTime)
    {
        _owner.ChangeState(ELidarObjectState.OnProgress);
        _owner.AddProgress(deltaTime);
    }

    public void OnScanLost()
    {
    }
}