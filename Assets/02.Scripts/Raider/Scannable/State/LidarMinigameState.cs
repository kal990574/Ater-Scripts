public class LidarMinigameState : ILidarScannableState
{
    private readonly LidarScannableObject _owner;

    public ELidarObjectState StateType => ELidarObjectState.OnMinigame;

    public LidarMinigameState(LidarScannableObject owner)
    {
        _owner = owner;
    }

    public void Enter()
    {
        _owner.BeginMinigame();
    }

    public void Exit()
    {
        _owner.EndMinigame();
    }

    public void Tick(float deltaTime)
    {
        _owner.TickMinigame(deltaTime);
    }

    public void OnScanning(float deltaTime)
    {
    }

    public void OnScanLost()
    {
        _owner.FailCurrentMinigame();
    }
}