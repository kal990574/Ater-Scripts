public class LidarMinigameState : ILidarTargetState
{
    private readonly LidarTarget _owner;

    public ELidarTargetState StateType => ELidarTargetState.OnMinigame;

    public LidarMinigameState(LidarTarget owner)
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
        _owner.ForceFailCurrentMinigame();
    }
}