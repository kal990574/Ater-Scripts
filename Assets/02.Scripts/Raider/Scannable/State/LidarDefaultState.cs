public class LidarDefaultState : ILidarScannableState
{
    private readonly LidarScannableObject _owner;

    public ELidarObjectState StateType => ELidarObjectState.Default;

    public LidarDefaultState(LidarScannableObject owner)
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
        _owner.ChangeState(ELidarObjectState.OnProgress);
        _owner.AddProgress(deltaTime);
    }

    public void OnScanLost()
    {
    }
}