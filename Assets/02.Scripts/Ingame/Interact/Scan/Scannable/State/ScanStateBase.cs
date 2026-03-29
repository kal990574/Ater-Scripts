public abstract class ScanStateBase : IScanState
{
    protected readonly ScannableObject Owner;

    public abstract EScanState StateType { get; }

    protected ScanStateBase(ScannableObject owner)
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
