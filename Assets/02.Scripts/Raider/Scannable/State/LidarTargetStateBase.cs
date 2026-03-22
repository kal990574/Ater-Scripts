public abstract class LidarTargetStateBase : ILidarTargetState
{
    protected readonly LidarTarget Owner;

    public abstract ELidarTargetState StateType { get; }

    protected LidarTargetStateBase(LidarTarget owner)
    {
        Owner = owner;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Tick(float deltaTime)
    {
    }

    public virtual void OnScanning(float deltaTime)
    {
    }

    public virtual void OnScanLost()
    {
    }
}
