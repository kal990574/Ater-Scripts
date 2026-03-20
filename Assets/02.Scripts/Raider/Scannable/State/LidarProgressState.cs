public class LidarProgressState : ILidarTargetState
{
    private readonly LidarTarget _owner;
    private float _elapsedTime;

    public ELidarTargetState StateType => ELidarTargetState.OnProgress;

    public LidarProgressState(LidarTarget owner)
    {
        _owner = owner;
    }

    public void Enter()
    {
        _elapsedTime = 0.0f;
    }

    public void Exit()
    {
    }

    public void Tick(float deltaTime)
    {
    }

    public void OnScanning(float deltaTime)
    {
        _elapsedTime += deltaTime;

        _owner.AddProgress(deltaTime);

        if (_owner.IsProgressComplete == true)
        {
            _owner.ChangeState(ELidarTargetState.OnCompleted);
            return;
        }

        if (_elapsedTime >= _owner.CurrentQTEDelay)
        {
            _owner.ChangeState(ELidarTargetState.OnPlayQTE);
        }
    }

    public void OnScanLost()
    {
        _owner.ChangeState(ELidarTargetState.OnReturn);
    }
}