public class LidarProgressState : ILidarScannableState
{
    private readonly LidarScannableObject _owner;

    public ELidarObjectState StateType => ELidarObjectState.OnProgress;

    public LidarProgressState(LidarScannableObject owner)
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
        _owner.AddProgress(deltaTime);
    }

    public void OnScanLost()
    {
        _owner.ChangeState(ELidarObjectState.OnReturn);
    }
}